// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// This is the main file containing the entry point of the project
// </summary>
// -----------------------------------------------------------------------

namespace BillingWebJob
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;   
    using System.Linq;    
    using BillingWebJob.AzureAnalyticsDb;
    using BillingWebJob.Helpers;
    using BillingWebJob.Models;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Rest;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;   

    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    
    /// <summary>
    /// Main class of the namespace.
    /// </summary>     
    public class Program
    {
        /// <summary>
        /// Azure WebJob method to be run.
        /// </summary>
        /// <param name="timer">Timing configuration of web job.</param>
        public static void CronJob(
            [TimerTrigger(typeof(Helpers.ConfigurableCronSchedule), RunOnStartup = true)] TimerInfo timer)
        {
            string errorMessage = string.Empty;
            int status = 0;
            int recordsCount = 0;
            string blobStorageUri = string.Empty;

            // determine type of customer
            string[] customerType = ConfigurationManager.AppSettings["BillingCustomerType"].Split(',');

            // definition of states
            // 0 => initialState,
            // 1 => databaseOperationCompletedSuccessfully
            // 2 => databaseAndStorageoperationCompletedSuccessfully
            // 3 => no data rows returned by API for given daterange
            // -1 => failure
            int i = 0;
            while (i < customerType.Length)
            {
                try
                {
                    // dbContext.Configuration.AutoDetectChangesEnabled = false;
                    Console.WriteLine("Job started for fetching billing data at " +
                                      DateTime.Now.ToString(CultureInfo.InvariantCulture));

                    Console.WriteLine("Starting job processing for Customer Type " + customerType[i]);

                    if (string.IsNullOrEmpty(customerType[i]))
                    {
                        Console.WriteLine(
                            "\nBillingCustomerType value is not provided in the web.config. Cannot proceed further without this input.");
                    }
                    else if (customerType[i].ToLower().Trim() == CustomerType.direct.ToString().ToLower())
                    {
                        // direct routine

                        // 1. get data from API
                        HttpOperationResponse<IList<UsageInfoModel>> billingRecordsFromApi;
                        string startDate = ConfigurationManager.AppSettings["BillingPeriodStartDate"];
                        string endDate = ConfigurationManager.AppSettings["BillingPeriodEndDate"];

                        billingRecordsFromApi = UserHelper.GetBillingDataFromApi(startDate, endDate);

                        if (billingRecordsFromApi.Body != null && billingRecordsFromApi.Body.Count > 0)
                        {
                            // 2. remove same dated data from sql "billing data" table
                            Console.WriteLine(billingRecordsFromApi.Body.Count +
                                              " data rows returned from the pricing api.");
                            Console.WriteLine(
                                "\nChecking for existing data in database which falls in same date range, these will be deleted.");
                            string message = UserHelper.DeleteExistingRecordsFordateRange(startDate, endDate);

                            Console.WriteLine(message);

                            // 3. update data in billingdata Table
                            Console.WriteLine("\nAdding new data rows in database..");
                            recordsCount = UserHelper.UpdateRecordsInDatabase(billingRecordsFromApi.Body);
                            status = 1;
                            Console.WriteLine("\n" + recordsCount +
                                              " records successfully appended to the database table: UserBillingData.");

                            // 4. keep a backup in Azure storage
                            Console.WriteLine("\nSaving backup of data in Azure Storage blob..");
                            blobStorageUri = UserHelper.UpdateRecordsInAzureStorage(billingRecordsFromApi.Body);
                            status = 2;

                            Console.WriteLine("\nData backup stored in Azure blob storage at :" + blobStorageUri);
                        }
                        else
                        {
                            Console.WriteLine(
                                "\n0 data rows returned from API for the given date range. No operation performed on DB and storage. ");
                            status = 3;
                        }
                    }
                    else if (customerType[i].ToLower().Trim() == CustomerType.csp.ToString().ToLower())
                    {
                        // csp routine
                        Console.WriteLine(
                            "Starting CSP Routine. Current Usage, Historic Usage and Historic Billing data will be updated.. ");

                        // 1. Call Csp records
                        status = recordsCount = 0;
                        blobStorageUri = null;
                        CspDataHelper.StartCspRoutine(out status, out recordsCount, out blobStorageUri);

                        // 2. Check for records count
                        if (recordsCount == 0)
                        {
                            status = 3;
                            Console.WriteLine("\nNo data rows returned by the APIs for the given date range.");
                        }
                        else
                        {
                            Console.WriteLine("\nA total of {0} rows added in the Db.", recordsCount);
                        }
                    }
                    else if (customerType[i].ToLower().Trim() == CustomerType.ea.ToString().ToLower())
                    {
                        // 1. Call Ea routine
                        status = recordsCount = 0;
                        blobStorageUri = null;
                        EaDataHelper.StartEaRoutine(out status, out recordsCount, out blobStorageUri);

                        // 2. Check for records count
                        if (recordsCount == 0)
                        {
                            status = 3;
                            Console.WriteLine("\nNo data rows returned by the APIs for the given date range.");
                        }
                        else
                        {
                            Console.WriteLine("\nA total of {0} rows added in the Db.", recordsCount);
                        }
                    }
                    else
                    {
                        Console.WriteLine(
                            "\nThe value provided for BillingCustomerType in web.config is not valid. Cannot proceed further without correct value.");
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    Exception tempException = ex;

                    while (tempException.InnerException != null)
                    {
                        errorMessage += "\nInnerException :\n" + tempException.InnerException.Message;
                        tempException = tempException.InnerException;
                    }

                    errorMessage += "\nStackTrace :\n" + ex.StackTrace;

                    if (status == 1)
                    {
                        Console.WriteLine("\nError encountered during azure storage backup operation: \n" + errorMessage);
                    }
                    else
                    {
                        status = -1;
                        recordsCount = 0;
                        Console.WriteLine("\nError encountered: \n" + errorMessage + " ");
                    }
                }
                finally
                {
                    Console.WriteLine("\nAdding audit information in AuditData table..\n");
                    using (AzureAnalyticsDbModel dbContext = new AzureAnalyticsDbModel())
                    {
                        dbContext.AuditDatas.Add(new AuditData
                        {
                            TimeStamp = DateTime.Now,
                            Status = status,
                            ErrorMessage = errorMessage,
                            RecordCount = recordsCount,
                            BlobStorageUrl = blobStorageUri,
                            CustomerType = customerType[i]
                        });
                        dbContext.SaveChanges();
                    }
                }

                i++;
            }

            Console.WriteLine("\nOperation complete. Exiting this run.\n");
        }

        /// <summary>
        /// Entry method of the Program class.
        /// </summary>
        internal static void Main()
        {
            ConfigHelper.TestConnections();
            JobHostConfiguration config = new JobHostConfiguration();
            config.UseTimers();
            JobHost host = new JobHost(config);
            host.RunAndBlock();
        }
    }
}