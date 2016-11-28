// -----------------------------------------------------------------------
// <copyright file="UserHelper.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// ----------------------------------------------------------------------- 

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingWebJob.AzureAnalyticsDb;
using BillingWebJob.Models;
using Microsoft.Rest;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BillingWebJob.Helpers
{
    /// <summary>
    /// Helps in User/Direct customer data management
    /// </summary>
    public static class UserHelper
    {
        private static BillingWebJob.AzureAnalyticsApi bd = new BillingWebJob.AzureAnalyticsApi();

        /// <summary>
        /// Get data from API for a start and end date
        /// </summary>
        /// <param name="startDate">start date for data fetch</param>
        /// <param name="endDate">end date for data fetch</param>
        /// <returns></returns>
        internal static HttpOperationResponse<IList<UsageInfoModel>> GetBillingDataFromApi(string startDate,
            string endDate)
        {
            HttpOperationResponse<IList<UsageInfoModel>> billingRecordsFromApi;
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                Console.WriteLine("Fetching pricing data in date range: " + startDate + " and " + endDate + "..");
                startDate = startDate.Split('-')[1] + "-" + startDate.Split('-')[0];
                endDate = endDate.Split('-')[1] + "-" + endDate.Split('-')[0];
                billingRecordsFromApi =
                    bd.UserBilling.GetDataForMonthRangeWithOperationResponseAsync(startDate, endDate).Result;
            }
            else
            {
                Console.WriteLine("Fetching pricing data for current month..");
                billingRecordsFromApi = bd.UserBilling.GetCurrentMonthDataWithOperationResponseAsync().Result;
            }

            return billingRecordsFromApi;
        }

        /// <summary>
        /// Remove existing user records from database
        /// </summary>
        /// <param name="startDate">start date for data fetch</param>
        /// <param name="endDate">end date for data fetch</param>
        /// <returns></returns>
        internal static string DeleteExistingRecordsFordateRange(string startDate, string endDate)
        {
            DateTime parsedStartTime, parsedEndTime;

            DateTime.TryParse(startDate, out parsedStartTime);
            DateTime.TryParse(endDate, out parsedEndTime);
            string message;

            List<UserBillingData> rowsToBeDeleted = null;
            using (AzureAnalyticsDbModel dbContext = new AzureAnalyticsDbModel())
            {
                if (parsedStartTime != null && parsedEndTime != null)
                {
                    rowsToBeDeleted =
                        dbContext.UserBillingDatas.Where(
                            x => x.EndTime.Value >= parsedStartTime && x.StartTime <= parsedEndTime).ToList();
                }

                if (rowsToBeDeleted != null && rowsToBeDeleted.Count > 0)
                {
                    dbContext.UserBillingDatas.RemoveRange(rowsToBeDeleted);
                    int result = dbContext.SaveChanges();
                    message = result + " rows found in the date range and have been removed from database. ";
                }
                else
                {
                    message = "No data found in db for the given date range. No records were deleted.";
                }
            }
            return message;
        }

        /// <summary>
        /// Create user backup data in Azure Blob Storage
        /// </summary>
        /// <param name="billingRecordsFromApi">records returned from API</param>
        /// <returns>Uri of the blob</returns>
        internal static string UpdateRecordsInAzureStorage(IList<UsageInfoModel> billingRecordsFromApi)
        {
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

            string flname = "UsageData_" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" +
                            DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".csv";

            // TODO: change naming convention once web api is converted to accepted params
            // string cont = subscription.Id + "\\" + stdate.Year.ToString() + "\\" + stdate.ToString("MM");
            // string flname = cont + "\\" + stdate.ToString("MMMM") + "From" + stdate.AddDays(-stdate.Day + 1).ToString("dd") + "To" + stdate.ToString("dd") + ".csv";

            string idval;
            // Create the container if it doesn't already exist.
            using (CsvFileWriter writer = new CsvFileWriter(resp))
            {
                foreach (var usg in billingRecordsFromApi)
                {
                    CsvRow row = new CsvRow();

                    if (usg != null)
                    {
                        idstarter = idstarter + 1;
                        idval = idstarter.ToString(CultureInfo.InvariantCulture);
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", idval));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.SubceriptionId));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.MeterName));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.MeteredRegion));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.MeterCategory));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.MeterSubCategory));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.UsageStartTime));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.UsageEndTime));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.MeteredService));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.MeteredServiceType));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.UserProject));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.Quantity.ToString()));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ItemTotal.ToString()));

                        writer.WriteRow(row);
                    }
                }
            }
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(flname);
            // using (var fileStream = System.IO.File.OpenRead(@"WriteTest.csv"))

            using (var fileStream = System.IO.File.OpenRead(resp))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            return blockBlob.SnapshotQualifiedUri.ToString();
        }

        /// <summary>
        /// Adding filtered Rows in the Database
        /// </summary>
        /// <param name="billingRecordsFromApi">User records obtaiend from API</param>
        /// <returns>The number of objects written to the underlying database</returns>
        public static int UpdateRecordsInDatabase(IList<UsageInfoModel> billingRecordsFromApi)
        {
            int recordsCount = 0;

            using (AzureAnalyticsDbModel dbContext = new AzureAnalyticsDbModel())
            {
                List<UserBillingData> recordsToBeAdded = new List<UserBillingData>();
                foreach (UsageInfoModel usageRecord in billingRecordsFromApi)
                {
                    DateTime parsedStartTime, parsedEndTime;
                    Decimal parsedQuantity, parsedTotal;

                    recordsToBeAdded.Add(new UserBillingData
                    {
                        EndTime =
                            DateTime.TryParse(usageRecord.UsageEndTime, out parsedEndTime)
                                ? parsedEndTime
                                : (DateTime?) null,
                        MeterCategory = usageRecord.MeterCategory,
                        MeterName = usageRecord.MeterName,
                        MeterService = usageRecord.MeteredService,
                        MeterType = usageRecord.MeteredServiceType,
                        MeterSubcategory = usageRecord.MeterSubCategory,
                        Project = usageRecord.UserProject,
                        Quantity =
                            decimal.TryParse(usageRecord.Quantity.ToString(), out parsedQuantity)
                                ? parsedQuantity
                                : (decimal?) null,
                        Region = usageRecord.MeteredRegion,
                        StartTime =
                            DateTime.TryParse(usageRecord.UsageStartTime, out parsedStartTime)
                                ? parsedStartTime
                                : (DateTime?) null,
                        SubscriptionId = usageRecord.SubceriptionId,
                        Total =
                            decimal.TryParse(usageRecord.ItemTotal.ToString(), out parsedTotal)
                                ? parsedTotal
                                : (decimal?) null,

                        ////TODO: extract and add code for info fields
                    });
                }

                dbContext.UserBillingDatas.AddRange(recordsToBeAdded);
                recordsCount = dbContext.SaveChanges();
            }
            return recordsCount;
        }
    }
}