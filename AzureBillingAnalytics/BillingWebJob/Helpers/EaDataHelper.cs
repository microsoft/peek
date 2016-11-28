// -----------------------------------------------------------------------
// <copyright file="EaDataHelper.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// This file contains the helper class for EA
// </summary>
// ----------------------------------------------------------------------- 

namespace BillingWebJob.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using BillingWebJob.AzureAnalyticsDb;
    using BillingWebJob.Models;
    using Microsoft.Rest;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Helps in CSP data management.
    /// </summary>
    public static class EaDataHelper
    {
        /// <summary>
        /// Object of API class to access it's methods.
        /// </summary>
        private static BillingWebJob.AzureAnalyticsApi azureAnalyticsApi = new BillingWebJob.AzureAnalyticsApi();

        /// <summary>
        /// Main EA Routine entry point.
        /// </summary>
        /// <param name="status">
        /// 0 => initialState,
        /// 1 => databaseOperationCompletedSuccessfully
        /// 2 => databaseAndStorageOperationCompletedSuccessfully
        /// 3 => no data rows returned by API for given dateRange
        /// -1 => failure.
        /// </param>
        /// <param name="totalRecordsCount">Number of records passed as reference, passed value is 0.</param>
        /// <param name="blobStorageUri">Uri of the blob used for backing up data.</param>
        public static void StartEaRoutine(out int status, out int totalRecordsCount, out string blobStorageUri)
        {
            // Initialising and declaring variables
            Console.WriteLine("\nEA Routine started..");
            totalRecordsCount = 0;
            status = 0;
            blobStorageUri = null;
            string blobFilename = null;
            HttpOperationResponse<IList<BillingDetailLineItem>> filteredRecords;
            DateTime batchStartDate, batchEndDate;
            List<BillingDetailLineItem> eaSummaryData;

            // Step 1. get Start and end dates from Config
            Console.WriteLine("Fetching dates from the config..");
            GetDates(out batchStartDate, out batchEndDate);

            if (batchEndDate >= batchStartDate)
            {
                Console.WriteLine(
                    "Fetching Ea Billing data from {0} to {1}",
                    batchStartDate.ToString(CultureInfo.InvariantCulture),
                    batchEndDate.ToString(CultureInfo.InvariantCulture));

                eaSummaryData = new List<BillingDetailLineItem>();

                // Step 2. loop through each month from Start Date to End Date
                while (batchEndDate >= batchStartDate)
                {
                    string date = batchStartDate.Month.ToString(CultureInfo.InvariantCulture) + "-" +
                                  batchStartDate.Year.ToString(CultureInfo.InvariantCulture);
                    Console.WriteLine("\nRoutine started for {0}", date);

                    // Step 3. Fetch Data to be added in DB (from the APIs if neceessary) for the current month
                    filteredRecords = EaDataHelper.GetFilteredEaBillingDataFromApi(date);

                    if (filteredRecords.Body != null && filteredRecords.Body.Count > 0)
                    {
                        // Step 4. Parse the raw data to List of EaBillingData
                        List<EaBillingData> parsedRecords = EaDataHelper.GetParsedEaRecords(filteredRecords);

                        // Step 5. Update data in EA Billing data Table
                        int recordsCount = EaDataHelper.UpdateEaRecordsInDatabase(parsedRecords);
                        totalRecordsCount += recordsCount;
                        status = 1;
                        Console.WriteLine(recordsCount +
                                          " new records successfully appended to the database table: EaBillingData.");

                        // Step 6. keep a backup in Azure storage
                        if (blobFilename == null)
                        {
                            blobFilename = "EaBillingData_" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" +
                                           DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" +
                                           DateTime.Now.Second + ".csv";
                        }

                        Console.WriteLine("Appending data for Blob Storage. It will be uploaded in the blob later");
                        eaSummaryData.AddRange(filteredRecords.Body.ToList());
                        //// Console.WriteLine("Data backup stored in Azure blob storage at :" + blobStorageUri);
                    }
                    else
                    {
                        Console.WriteLine(
                            "No changes made in Db for this month..",
                            batchStartDate.Month,
                            batchStartDate.Year);
                    }

                    batchStartDate = batchStartDate.AddMonths(1);
                }

                if (blobFilename != null && eaSummaryData.Count > 0)
                {
                    blobStorageUri = UpdateEaRecordsInAzureStorage(eaSummaryData, blobFilename);
                    status = 2;
                    Console.WriteLine("Data backup stored in Azure blob storage at :" + blobStorageUri);
                }
                else
                {
                    status = 2;
                    Console.WriteLine("No Data to backup in Azure storage");
                }
            }
            else
            {
                Console.WriteLine("End date is greater than Start date. Please Check..");
            }

            status = 1;
        }

        /// <summary>
        /// Parsing the obtained Filtered records from httpResponse to the list of EABillingData Type.
        /// </summary>
        /// <param name="eaBillingRecordsFromApi">List of data returned from API.</param>
        /// <returns>List of type EABillingData.</returns>
        private static List<EaBillingData> GetParsedEaRecords(
            HttpOperationResponse<IList<BillingDetailLineItem>> eaBillingRecordsFromApi)
        {
            Console.WriteLine(eaBillingRecordsFromApi.Body.Count + " data rows returned from the ea billing api.");
            List<string> invoicesFound = eaBillingRecordsFromApi.Body.Select(x => x.DownloadUrl).Distinct().ToList();

            List<EaBillingData> eaBillingRecordsToBeAppendedInDb = new List<EaBillingData>();
            foreach (string invoice in invoicesFound)
            {
                List<BillingDetailLineItem> itemsForInvoice =
                    eaBillingRecordsFromApi.Body.Where(x => x.DownloadUrl == invoice).ToList();
                foreach (BillingDetailLineItem relevantLineItem in itemsForInvoice)
                {
                    eaBillingRecordsToBeAppendedInDb.Add(
                        new EaBillingData
                        {
                            AccountName = relevantLineItem.AccountName,
                            AccountOwnerId = relevantLineItem.AccountOwnerId,
                            AdditionalInfo = relevantLineItem.AdditionalInfo,
                            Component = relevantLineItem.Component,
                            CostCenter = relevantLineItem.CostCenter,
                            Date = relevantLineItem.Date.Value.Date,
                            DepartmentName = relevantLineItem.DepartmentName,
                            Day = relevantLineItem.Day,
                            ExtendedCost = float.Parse(relevantLineItem.ExtendedCost, CultureInfo.InvariantCulture),
                            Key = relevantLineItem.Key,
                            Month = relevantLineItem.Month,
                            Product = relevantLineItem.Product,
                            ResourceGUID = relevantLineItem.ResourceGUID,
                            ResourceKey = relevantLineItem.ResourceKey,
                            ResourceQtyConsumed =
                                float.Parse(relevantLineItem.ResourceQtyConsumed, CultureInfo.InvariantCulture),
                            ResourceRate = float.Parse(relevantLineItem.ResourceRate, CultureInfo.InvariantCulture),
                            Service = relevantLineItem.Service,
                            DownloadUrl = relevantLineItem.DownloadUrl,
                            ServiceAdministratorId = relevantLineItem.ServiceAdministratorId,
                            ServiceInfo = relevantLineItem.ServiceInfo,
                            ServiceInfo1 = relevantLineItem.ServiceInfo1,
                            ServiceInfo2 = relevantLineItem.ServiceInfo2,
                            ServiceRegion = relevantLineItem.ServiceRegion,
                            ServiceResource = relevantLineItem.ServiceResource,
                            ServiceSubRegion = relevantLineItem.ServiceSubRegion,
                            ServiceType = relevantLineItem.ServiceType,
                            StoreServiceIdentifier = relevantLineItem.StoreServiceIdentifier,
                            SubscriptionGuid = relevantLineItem.SubscriptionGuid,
                            SubscriptionId = relevantLineItem.SubscriptionId,
                            SubscriptionName = relevantLineItem.SubscriptionName,
                            Tags = relevantLineItem.Tags,
                            Year = relevantLineItem.Year
                        });
                }
            }

            return eaBillingRecordsToBeAppendedInDb;
        }

        /// <summary>
        /// Business Logic to obtain the data rows to be inserted (if any) in the Database.
        /// </summary>
        /// <param name="date">Month-year date for data collection.</param>
        /// <returns>List of data returned from API.</returns>
        private static HttpOperationResponse<IList<BillingDetailLineItem>> GetFilteredEaBillingDataFromApi(string date)
        {
            HttpOperationResponse<IList<BillingDetailLineItem>> eaBillingRecordsFromApi;

            int month = int.Parse(date.Split('-')[0], CultureInfo.InvariantCulture);
            int year = int.Parse(date.Split('-')[1], CultureInfo.InvariantCulture);

            using (AzureAnalyticsDbModel dbContext = new AzureAnalyticsDbModel())
            {
                List<EaBillingData> itemsFromDatabase;
                int recordsNums;

                // Checking records in Database
                Console.WriteLine("Checking Db for existing records for the month {0}-{1}..", month, year);
                itemsFromDatabase = dbContext.EaBillingDatas.Where(x => x.Month == month && x.Year == year).ToList();
                recordsNums = itemsFromDatabase.Count();

                if (recordsNums == 0)
                {
                    // Call API
                    Console.WriteLine("No existing records found in Database for this month. Calling API for the data..");
                    eaBillingRecordsFromApi =
                        azureAnalyticsApi.EaBilling.GetSingleMonthDataWithOperationResponseAsync(date).Result;
                }
                else
                {
                    // Records Found
                    Console.WriteLine("{0} records found.", recordsNums);
                    if (ConfigurationManager.AppSettings["SanityCheck"] == "0")
                    {
                        eaBillingRecordsFromApi = new HttpOperationResponse<IList<BillingDetailLineItem>>();
                    }
                    else if (ConfigurationManager.AppSettings["SanityCheck"] == "1")
                    {
                        Console.WriteLine("Deleting the existing rows for month {0}-{1}..", month, year);
                        dbContext.EaBillingDatas.RemoveRange(itemsFromDatabase);
                        int result = dbContext.SaveChanges();
                        Console.WriteLine(
                            "Successfully deleted {0} Rows for month {1}-{2} from the Database", 
                            result,
                            month, 
                            year);
                        Console.WriteLine("Calling API for current month's data.. ");
                        eaBillingRecordsFromApi =
                            azureAnalyticsApi.EaBilling.GetSingleMonthDataWithOperationResponseAsync(date).Result;
                        if (eaBillingRecordsFromApi.Body == null)
                        {
                            Console.WriteLine("No data obtained from the APIs for this month..");
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid value of SanityCheck");
                    }
                }

                return eaBillingRecordsFromApi;
            }
        }

        /// <summary>
        /// Adding filtered Rows in the Database.
        /// </summary>
        /// <param name="newEaRecords">Adding the EA records into database.</param>
        /// <returns>The number of objects written to the underlying database.</returns>
        private static int UpdateEaRecordsInDatabase(List<EaBillingData> newEaRecords)
        {
            int recordsCount = 0;
            using (AzureAnalyticsDbModel dbContext = new AzureAnalyticsDbModel())
            {
                Console.WriteLine("Adding Rows in Database..");
                dbContext.EaBillingDatas.AddRange(newEaRecords);
                recordsCount = dbContext.SaveChanges();
                ////foreach (var record in newEaRecords)
                ////{
                ////    dbContext.EaBillingDatas.Add(record);
                ////    recordsCount = dbContext.SaveChanges();
                ////}
            }

            return recordsCount;
        }

        /// <summary>
        /// For getting the start and end date for fetching data from config file.
        /// </summary>
        /// <param name="batchStartDate">Start date for collection.</param>
        /// <param name="batchEndDate">End date for collection.</param>
        private static void GetDates(out DateTime batchStartDate, out DateTime batchEndDate)
        {
            // Get dates from the config and string and parse as date type objects
            string startDate = ConfigurationManager.AppSettings["EaBillingPeriodStartDate"];
            string endDate = ConfigurationManager.AppSettings["EaBillingPeriodEndDate"];

            int startYear = 0, startMonth = 0, endYear = 0, endMonth = 0;

            if (!string.IsNullOrEmpty(startDate))
            {
                int.TryParse(startDate.Split('-')[0], out startYear);
                int.TryParse(startDate.Split('-')[1], out startMonth);
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                int.TryParse(endDate.Split('-')[0], out endYear);
                int.TryParse(endDate.Split('-')[1], out endMonth);
            }

            if (startYear != 0 && startMonth != 0 && endMonth != 0 && endYear != 0)
            {
                batchStartDate = new DateTime(startYear, startMonth, 1);
                batchEndDate = new DateTime(endYear, endMonth, 1);
            }
            else if (startYear != 0 && startMonth != 0 && endMonth == 0 && endYear == 0)
            {
                batchStartDate = new DateTime(startYear, startMonth, 1);
                batchEndDate = DateTime.Now;
            }
            else
            {
                throw new Exception("Invalid Dates in the config");
            }
        }

        /// <summary>
        /// Create EA backup data in Azure Blob Storage.
        /// </summary>
        /// <param name="filteredRecords">List of billing records of EA.</param>
        /// <param name="filename">File name for blob storage.</param>
        /// <returns>Uri of the blob.</returns>
        private static string UpdateEaRecordsInAzureStorage(List<BillingDetailLineItem> filteredRecords, string filename)
        {
            Console.WriteLine("Taking backup on Azure..");

            // connect to the storage account:
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Retrieve storage account from connection string.

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("billingdata");
            container.CreateIfNotExists();

            container.SetPermissions(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            int idstarter = 0;
            string resp = Path.GetTempPath() + "csvtep.txt";

            //// TODO: change naming convention once web api is converted to accepted params
            //// string cont = subscription.Id + "\\" + stdate.Year.ToString() + "\\" + stdate.ToString("MM");
            //// string flname = cont + "\\" + stdate.ToString("MMMM") + "From" + stdate.AddDays(-stdate.Day + 1).ToString("dd") + "To" + stdate.ToString("dd") + ".csv";

            string idval;

            // Create the container if it doesn't already exist.
            using (CsvFileWriter writer = new CsvFileWriter(resp))
            {
                foreach (var rcd in filteredRecords)
                {
                    CsvRow row = new CsvRow();

                    if (rcd != null)
                    {
                        idstarter = idstarter + 1;
                        idval = idstarter.ToString(CultureInfo.InvariantCulture);
                        row.Add(string.Format(CultureInfo.InvariantCulture, "CultureInfo.InvariantCulture, {0}", idval));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.AccountName));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.AccountOwnerId));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.AdditionalInfo));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.Component));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.CostCenter));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.Date.ToString()));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.Day.ToString()));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.DepartmentName));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.DownloadUrl));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.ExtendedCost));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "CultureInfo.InvariantCulture, {0}", rcd.Key));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.Month.ToString()));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.Product));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.ResourceGUID));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.ResourceKey));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.ResourceQtyConsumed));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.ResourceRate));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.Service));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.ServiceAdministratorId));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.ServiceInfo));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.ServiceInfo1));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.ServiceInfo2));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.ServiceRegion));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.ServiceResource));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.ServiceSubRegion));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.ServiceType));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.StoreServiceIdentifier));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.SubscriptionGuid));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.SubscriptionId));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.SubscriptionName));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.Tags));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", rcd.Year.ToString()));

                        writer.WriteRow(row);
                    }
                }
            }

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);
            //// using (var fileStream = System.IO.File.OpenRead(@"WriteTest.csv"))

            using (var fileStream = System.IO.File.OpenRead(resp))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            return blockBlob.SnapshotQualifiedUri.ToString();
        }
    }
}