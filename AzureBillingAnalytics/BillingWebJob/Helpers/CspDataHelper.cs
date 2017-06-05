// -----------------------------------------------------------------------
// <copyright file="CspDataHelper.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// This file contains the helper class for CSP
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
    using AzureAnalyticsDb;
    using BillingWebJob.Models;
    using Microsoft.Rest;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Newtonsoft.Json;
    using System.Data.SqlClient;
    using System.Data;
    using System.Data.Entity.Infrastructure;

    /// <summary>
    /// Helps in CSP data management.
    /// </summary>
    internal static class CspDataHelper
    {
        /// <summary>
        /// Object of API class to access it's methods.
        /// </summary>
        internal static BillingWebJob.BillingDataApi AzureAnalyticsApi = new BillingWebJob.BillingDataApi();

        /// <summary>
        /// Parse the billing data collected from API into a custom class.
        /// </summary>
        /// <param name="cspBillingRecordsFromApi">List of summary data returned from API.</param>
        /// <returns>List of type CSPBillingData.</returns>
        internal static List<CspBillingData> GetParsedCspBillingRecords(
            HttpOperationResponse<IList<CspUsageLineItem>> cspBillingRecordsFromApi)
        {
            // Console.WriteLine(cspBillingRecordsFromApi.Body.Count + " data rows returned from the csp billing api.");
            List<string> invoicesFound = cspBillingRecordsFromApi.Body.Select(x => x.InvoiceNumber).Distinct().ToList();

            List<CspBillingData> cspBillingRecordsToBeAppendedInDb = new List<CspBillingData>();
            foreach (string invoice in invoicesFound)
            {
                List<CspUsageLineItem> itemsForInvoice =
                    cspBillingRecordsFromApi.Body.Where(x => x.InvoiceNumber == invoice).ToList();
                //// Console.WriteLine("New Invoice fetched : " + invoice + ". Will be appended in DB");

                foreach (CspUsageLineItem relevantLineItem in itemsForInvoice)
                {
                    cspBillingRecordsToBeAppendedInDb.Add(
                        new CspBillingData
                        {
                            BillingProvider = relevantLineItem.BillingProvider,
                            ChargeEndDate = (DateTime?)relevantLineItem.ChargeEndDate.Value,
                            ChargeStartDate = (DateTime?)relevantLineItem.ChargeStartDate.Value,
                            ConsumedQuantity = relevantLineItem.ConsumedQuantity,
                            CustomerBillableAccount = relevantLineItem.CustomerBillableAccount,
                            CustomerCompanyName = relevantLineItem.CustomerCompanyName,
                            InvoiceNumber = relevantLineItem.InvoiceNumber,
                            MpnId = relevantLineItem.MpnId,
                            OrderId = relevantLineItem.OrderId,
                            PartnerBillableAccountId = relevantLineItem.PartnerBillableAccountId,
                            PartnerId = relevantLineItem.PartnerId,
                            PartnerName = relevantLineItem.PartnerName,
                            Region = relevantLineItem.Region,
                            ResourceGuid = relevantLineItem.ResourceGuid,
                            ResourceName = relevantLineItem.ResourceName,
                            ServiceName = relevantLineItem.ServiceName,
                            ServiceType = relevantLineItem.ServiceType,
                            SubscriptionDescription = relevantLineItem.SubscriptionDescription,
                            SubscriptionId = relevantLineItem.SubscriptionId,
                            SubscriptionName = relevantLineItem.SubscriptionName,
                            TiermpnId = relevantLineItem.TierMpnId,
                            UsageDate = (DateTime?)relevantLineItem.UsageDate.Value
                        });
                }
            }

            return cspBillingRecordsToBeAppendedInDb;
        }

        /// <summary>
        /// Parse the summary data collected from API into a custom class.
        /// </summary>
        /// <param name="cspSummaryRecordsFromApi">List of summary data returned from API.</param>
        /// <returns>List of type CSPBillingData.</returns>
        internal static List<CspSummaryData> GetParsedCspSummaryRecords(
            HttpOperationResponse<IList<UsageBasedLineItem>> cspSummaryRecordsFromApi)
        {
            //// Console.WriteLine(cspSummaryRecordsFromApi.Body.Count + " data rows returned from the csp billing summary api.");

            List<string> invoicesFound = cspSummaryRecordsFromApi.Body.Select(x => x.InvoiceNumber).Distinct().ToList();

            List<CspSummaryData> cspBillingRecordsToBeAppendedInDb = new List<CspSummaryData>();
            foreach (string invoice in invoicesFound)
            {
                List<UsageBasedLineItem> itemsForInvoice =
                    cspSummaryRecordsFromApi.Body.Where(x => x.InvoiceNumber == invoice).ToList();
                //// Console.WriteLine("New Invoice fetched : " + invoice + ". Will be appended in DB");

                foreach (UsageBasedLineItem relevantLineItem in itemsForInvoice)
                {
                    cspBillingRecordsToBeAppendedInDb.Add(
                        new CspSummaryData
                        {
                            ChargeEndDate = (DateTime?)relevantLineItem.ChargeEndDate.Value,
                            ChargeStartDate = (DateTime?)relevantLineItem.ChargeStartDate.Value,
                            ChargeType = relevantLineItem.ChargeType,
                            ConsumedQuantity = relevantLineItem.ConsumedQuantity,
                            ConsumptionDiscount = relevantLineItem.ConsumptionDiscount,
                            ConsumptionPrice = relevantLineItem.ConsumptionPrice,
                            Currency = relevantLineItem.Currency,
                            CustomerCompanyName = relevantLineItem.CustomerCompanyName,
                            DetailLineItemId = relevantLineItem.DetailLineItemId,
                            IncludedQuantity = relevantLineItem.IncludedQuantity,
                            InvoiceNumber = relevantLineItem.InvoiceNumber,
                            ListPrice = relevantLineItem.ListPrice,
                            MpnId = relevantLineItem.MpnId,
                            OrderId = relevantLineItem.OrderId,
                            OverageQuantity = relevantLineItem.OverageQuantity,
                            PartnerBillingAccountId = relevantLineItem.PartnerBillableAccountId,
                            PartnerId = relevantLineItem.PartnerId,
                            PartnerName = relevantLineItem.PartnerName,
                            PostTaxEffectiveRate = relevantLineItem.PostTaxEffectiveRate,
                            PostTaxTotal = relevantLineItem.PostTaxTotal,
                            PreTaxCharges = relevantLineItem.PretaxCharges,
                            PreTaxEffectiveRate = relevantLineItem.PretaxEffectiveRate,
                            Region = relevantLineItem.Region,
                            ResourceGuid = relevantLineItem.ResourceGuid,
                            ResourceName = relevantLineItem.ResourceName,
                            ServiceName = relevantLineItem.ServiceName,
                            ServiceType = relevantLineItem.ServiceType,
                            Sku = relevantLineItem.Sku,
                            SubscriptionDescription = relevantLineItem.SubscriptionDescription,
                            SubscriptionId = relevantLineItem.SubscriptionId,
                            SubscriptionName = relevantLineItem.SubscriptionName,
                            TaxAmount = relevantLineItem.TaxAmount,
                            Tier2MpnId = relevantLineItem.Tier2MpnId
                        });
                }
            }

            return cspBillingRecordsToBeAppendedInDb;
        }

        /// <summary>
        /// Writes data collected into the database specified in the config file element AzureAnalyticsDbModel under attribute initial catalog.
        /// </summary>
        /// <param name="cspUsageRecordsFromApi">Contains a collection of CSP usage records.</param>
        /// <returns>The number of objects written to the underlying database.</returns>
        internal static int UpdateCurrentUsageRecordsInDb(IList<CspAzureResourceUsageRecord> cspUsageRecordsFromApi)
        {
            Console.WriteLine("\nAll Usage data which exist in DB will be deleted and replaced by new line items.");
            int count;
            using (AzureAnalyticsDbModel dbContext = new AzureAnalyticsDbModel())
            {
                List<CspUsageData> itemsFromDatabase = dbContext.CspUsageDatas.ToList();
                Console.WriteLine("\n" + itemsFromDatabase.Count() + " usage records exist in DB and will be deleted.");

                if (itemsFromDatabase.Count() > 0)
                {
                    dbContext.CspUsageDatas.RemoveRange(itemsFromDatabase);
                    dbContext.SaveChanges();
                }

                List<CspUsageData> newItemsForDatabase = new List<CspUsageData>();

                foreach (CspAzureResourceUsageRecord usageRecord in cspUsageRecordsFromApi)
                {
                    newItemsForDatabase.Add(
                        new CspUsageData
                        {
                            BillingEndDate = (DateTime?)usageRecord.BillingEndDate.Value,
                            BillingStartDate = (DateTime?)usageRecord.BillingStartDate.Value,
                            Category = usageRecord.Category,
                            CustomerCommerceId = usageRecord.CustomerCommerceId,
                            CustomerDomain = usageRecord.CustomerDomain,
                            CustomerId = usageRecord.CustomerId,
                            CustomerName = usageRecord.CustomerName,
                            CustomerRelationshipToPartner = usageRecord.CustomerRelationshipToPartner,
                            CustomerTenantId = usageRecord.CustomerTenantId,
                            QuantityUsed = usageRecord.QuantityUsed,
                            ResourceId = usageRecord.ResourceId,
                            ResourceName = usageRecord.ResourceName,
                            SubCategory = usageRecord.Subcategory,
                            SubscriptionContractType = usageRecord.SubscriptionContractType,
                            SubscriptionId = usageRecord.SubscriptionId,
                            SubscriptionName = usageRecord.SubscriptionName,
                            SubscriptionStatus = usageRecord.SubscriptionStatus,
                            TotalCost = usageRecord.TotalCost,
                            Unit = usageRecord.Unit
                        });
                }

                Console.WriteLine("\n" + newItemsForDatabase.Count() +
                                  " new usage records will be added to the database.");

                dbContext.CspUsageDatas.AddRange(newItemsForDatabase);
                count = dbContext.SaveChanges();
            }

            return count;
        }

        /// <summary>
        /// Create CSP billing backup data in Azure Blob Storage.
        /// </summary>
        /// <param name="billingRecords">List of billing records of CSP.</param>
        /// <param name="blobFilename">File name for blob storage.</param>
        /// <returns>Uri of the blob.</returns>
        internal static string UpdateCspRecordsInAzureStorage(List<CspUsageLineItem> billingRecords, string blobFilename)
        {
            //// connect to the storage account:
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["StorageConnectionString"]);
            //// Retrieve storage account from connection string.
            //// Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("billingcspdata");
            container.CreateIfNotExists();

            container.SetPermissions(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            int idstarter = 0;
            string resp = Path.GetTempPath() + "csvtep.txt";

            //// TODO: change naming convention once web api is converted to accepted params
            //// string cont = subscription.Id + "\\" + stdate.Year.ToString() + "\\" + stdate.ToString("MM");
            //// string blobFilename = cont + "\\" + stdate.ToString("MMMM") + "From" + stdate.AddDays(-stdate.Day + 1).ToString("dd") + "To" + stdate.ToString("dd") + ".csv";

            string idval;
            //// Create the container if it doesn't already exist.
            using (CsvFileWriter writer = new CsvFileWriter(resp))
            {
                foreach (var usg in billingRecords)
                {
                    CsvRow row = new CsvRow();
                    if (usg != null)
                    {
                        idstarter = idstarter + 1;
                        idval = idstarter.ToString(CultureInfo.InvariantCulture);
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", idval));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.BillingProvider));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ChargeEndDate));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ChargeStartDate));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ConsumedQuantity));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.CustomerBillableAccount));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.CustomerCompanyName));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.InvoiceNumber));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.MpnId));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.OrderId));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.PartnerBillableAccountId));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.PartnerId));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.PartnerName));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.Region));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ResourceGuid));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ResourceName));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ServiceName));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ServiceType));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.SubscriptionDescription));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.SubscriptionId));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.SubscriptionName));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.TierMpnId));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.UsageDate));
                        writer.WriteRow(row);
                    }
                }
            }

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobFilename);

            //// using (var fileStream = System.IO.File.OpenRead(@"WriteTest.csv"))

            using (var fileStream = System.IO.File.OpenRead(resp))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            return blockBlob.SnapshotQualifiedUri.ToString();
        }

        /// <summary>
        /// Creating backup of data collected in azure storage blob.
        /// </summary>
        /// <param name="billingRecords">Records of CSP billing.</param>
        /// <param name="blobFilename">Name of blob file.</param>
        /// <returns>Uri of the blob written data to.</returns>
        internal static string UpdateCspSummaryRecordsInAzureStorage(List<UsageBasedLineItem> billingRecords, string blobFilename)
        {
            //// connect to the storage account:
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["StorageConnectionString"]);
            //// Retrieve storage account from connection string.
            //// Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("billingcspdata");
            container.CreateIfNotExists();

            container.SetPermissions(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            int idstarter = 0;
            string resp = Path.GetTempPath() + "csvtep.txt";

            //// TODO: change naming convention once web api is converted to accepted params
            //// string cont = subscription.Id + "\\" + stdate.Year.ToString() + "\\" + stdate.ToString("MM");
            //// string blobFilename = cont + "\\" + stdate.ToString("MMMM") + "From" + stdate.AddDays(-stdate.Day + 1).ToString("dd") + "To" + stdate.ToString("dd") + ".csv";

            string idval;
            
            // Create the container if it doesn't already exist.
            using (CsvFileWriter writer = new CsvFileWriter(resp))
            {
                foreach (var usg in billingRecords)
                {
                    CsvRow row = new CsvRow();
                    if (usg != null)
                    {
                        idstarter = idstarter + 1;
                        idval = idstarter.ToString(CultureInfo.InvariantCulture);
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", idval));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.Attributes));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.BillingProvider));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ChargeEndDate));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ChargeStartDate));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ChargeType));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ConsumedQuantity));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ConsumptionDiscount));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ConsumptionPrice));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.Currency));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.CustomerCompanyName));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.DetailLineItemId));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.IncludedQuantity));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.InvoiceLineItemType));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.InvoiceNumber));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ListPrice));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.MpnId));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.OrderId));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.OverageQuantity));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.PartnerBillableAccountId));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.PartnerId));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.PartnerName));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.PostTaxEffectiveRate));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.PostTaxTotal));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.PretaxCharges));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.PretaxEffectiveRate));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.Region));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ResourceGuid));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ResourceName));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ServiceName));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.ServiceType));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.Sku));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.SubscriptionDescription));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.SubscriptionId));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.SubscriptionName));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.TaxAmount));
                        row.Add(string.Format(CultureInfo.InvariantCulture, "{0}", usg.Tier2MpnId));

                        writer.WriteRow(row);
                    }
                }
            }

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobFilename);

            //// using (var fileStream = System.IO.File.OpenRead(@"WriteTest.csv"))
            using (var fileStream = System.IO.File.OpenRead(resp))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            return blockBlob.SnapshotQualifiedUri.ToString();
        }

        /// <summary>
        /// Update the database with the new values of CSP Records.
        /// </summary>
        /// <param name="newCspRecords">List of new CSP billing records.</param>
        /// <param name="newCspSummaryRecords">List of new CSP summary records.</param>
        /// <returns>The number of objects written to the underlying database.</returns>
        internal static int UpdateCspRecordsInDatabase(
            IList<CspBillingData> newCspRecords,
            IList<CspSummaryData> newCspSummaryRecords)
        {
            int recordsCount = 0;

            using (AzureAnalyticsDbModel dbContext = new AzureAnalyticsDbModel())
            {
                dbContext.CspBillingDatas.AddRange(newCspRecords);
                dbContext.CspSummaryDatas.AddRange(newCspSummaryRecords);
                recordsCount = dbContext.SaveChanges();
            }

            return recordsCount;
        }

        /// <summary>
        /// Check Db and get billing and summary data from API, using sanity check.
        /// </summary>
        /// <param name="date">Date from which data to be fetched.</param>
        /// <param name="cspSummaryRecordsFromApi">Summary data collected from API. Sent as null. Passed as reference.</param>
        /// <param name="cspBillingRecordsFromApi">Billing data collected from API for a month. Sent as null. Passed as reference.</param>
        internal static void GetFilteredCspBillingDataFromApi(
            string date,
            out HttpOperationResponse<IList<UsageBasedLineItem>> cspSummaryRecordsFromApi,
            out HttpOperationResponse<IList<CspUsageLineItem>> cspBillingRecordsFromApi)
        {
            int month = int.Parse(date.Split('-')[0], CultureInfo.InvariantCulture);
            int year = int.Parse(date.Split('-')[1], CultureInfo.InvariantCulture);

            using (AzureAnalyticsDbModel dbContext = new AzureAnalyticsDbModel())
            {
                List<CspSummaryData> itemsFromDatabase;
                int recordsNums;

                // Checking records in Database
                Console.WriteLine("Checking Db for existing csp billing records for the month {0}-{1}..", month, year);
                itemsFromDatabase =
                    dbContext.CspSummaryDatas.Where(
                        x => x.ChargeEndDate.Value.Month == month && x.ChargeEndDate.Value.Year == year).ToList();
                recordsNums = itemsFromDatabase.Count();

                if (recordsNums == 0)
                {
                    // Call API
                    Console.WriteLine("No existing records found in Database for this month. Calling API for the data..");
                    cspSummaryRecordsFromApi =
                        AzureAnalyticsApi.CspSummary.GetSingleMonthDataWithHttpMessagesAsync(date).Result;
                    cspBillingRecordsFromApi =
                        AzureAnalyticsApi.CspBilling.GetSingleMonthDataWithHttpMessagesAsync(date).Result;
                    Console.WriteLine(
                        "{0} data rows returned from the csp summary api.",
                        cspSummaryRecordsFromApi.Body.Count);
                    Console.WriteLine(
                        "{0} data rows returned from the csp billing api.",
                        cspBillingRecordsFromApi.Body.Count);
                }
                else
                {
                    // Records Found
                    Console.WriteLine("{0} matching records found in db in CspSummaryData table.", recordsNums);
                    if (ConfigurationManager.AppSettings["SanityCheck"] == "0")
                    {
                        cspSummaryRecordsFromApi = new HttpOperationResponse<IList<UsageBasedLineItem>>();
                        cspBillingRecordsFromApi = new HttpOperationResponse<IList<CspUsageLineItem>>();
                        Console.WriteLine("No records appended in database.");
                    }
                    else if (ConfigurationManager.AppSettings["SanityCheck"] == "1")
                    {
                        // Console.WriteLine("Deleting the existing rows for {0}-{1}..", Month, Year);
                        dbContext.CspSummaryDatas.RemoveRange(itemsFromDatabase);
                        Console.WriteLine(
                            "Deleting {0} existing rows for {1}-{2} from table: CspSummaryData..",
                            itemsFromDatabase.Count, 
                            month, 
                            year);

                        var billingItemsFromDatabase =
                            dbContext.CspBillingDatas.Where(
                                    x => x.ChargeEndDate.Value.Month == month && x.ChargeEndDate.Value.Year == year)
                                .ToList();
                        dbContext.CspBillingDatas.RemoveRange(billingItemsFromDatabase);
                        Console.WriteLine(
                            "Deleting {0} existing rows for {1}-{2} from table: CspBillingData..",
                            billingItemsFromDatabase.Count, 
                            month, 
                            year);

                        int result = dbContext.SaveChanges();
                        Console.WriteLine("Successfully deleted {0} Rows.", result, month, year);
                        Console.WriteLine("Calling API for current month's data.. ");
                        cspSummaryRecordsFromApi =
                            AzureAnalyticsApi.CspSummary.GetSingleMonthDataWithHttpMessagesAsync(date).Result;
                        cspBillingRecordsFromApi =
                            AzureAnalyticsApi.CspBilling.GetSingleMonthDataWithHttpMessagesAsync(date).Result;
                        Console.WriteLine(
                            "{0} data rows returned from the csp summary api.",
                            cspSummaryRecordsFromApi.Body.Count);
                        Console.WriteLine(
                            "{0} data rows returned from the csp billing api.",
                            cspBillingRecordsFromApi.Body.Count);

                        if (cspSummaryRecordsFromApi.Body == null)
                        {
                            Console.WriteLine("No data obtained from the APIs for month {0}-{1}", month, year);
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid value of SanityCheck");
                    }
                }
            }
        }

        /// <summary>
        /// Entry point of CSP. 
        /// </summary>
        /// <param name="status">
        /// Definition of states
        /// 0 => initialState
        /// 1 => databaseOperationCompletedSuccessfully
        /// 2 => databaseAndStorageOperationCompletedSuccessfully
        /// 3 => no data rows returned by API for given dateRange
        /// -1 => failure.
        /// </param>
        /// <param name="totalRecordsCount">Number of records passed as reference, passed value is 0.</param>
        /// <param name="blobStorageUri">Uri of the blob used for backing up data.</param>
        internal static void StartCspRoutine(out int status, out int totalRecordsCount, out string blobStorageUri)
        {
            // current usage
            totalRecordsCount = 0;
            status = 0;
            blobStorageUri = null;
            string blobFilename = null;
            List<CspUsageLineItem> cspAggregateData;

            Console.WriteLine(
                "\nFetching records for Current Month's Usage from the API. This may take a while. If this operation is timimg out, consider increasing the TimeOut limit in Configuration file. ");

            HttpOperationResponse<IList<CspAzureResourceUsageRecord>> cspUsageRecordsFromApi =
                AzureAnalyticsApi.CspUsage.GetAllDataWithHttpMessagesAsync().Result;
            Console.WriteLine("\n" + cspUsageRecordsFromApi.Body.Count + " records fetched from the API.. ");

            if (cspUsageRecordsFromApi.Body != null && cspUsageRecordsFromApi.Body.Count > 0)
            {
                totalRecordsCount += CspDataHelper.UpdateCurrentUsageRecordsInDb(cspUsageRecordsFromApi.Body);
            }

            // Historic Usage and Billing
            Console.WriteLine("\nNow fetching historic usage and billing records from the API month-by-month.");
            HttpOperationResponse<IList<UsageBasedLineItem>> filteredCspSummaryRecords =
                new HttpOperationResponse<IList<UsageBasedLineItem>>();
            HttpOperationResponse<IList<CspUsageLineItem>> filteredCspBillingRecords =
                new HttpOperationResponse<IList<CspUsageLineItem>>();
            DateTime batchStartDate, batchEndDate;

            // Step 1. get Start and end dates from Config
            Console.WriteLine("\nFetching dates from the config..");
            GetDates(out batchStartDate, out batchEndDate);

            if (batchEndDate >= batchStartDate)
            {
                Console.WriteLine(
                    "\nFetching Csp Billing and Summary data from {0} to {1}",
                    batchStartDate.ToString(CultureInfo.InvariantCulture),
                    batchEndDate.ToString(CultureInfo.InvariantCulture));
                cspAggregateData = new List<CspUsageLineItem>();

                // Step 2. loop through each month from Start Date to End Date
                while (batchEndDate >= batchStartDate)
                {
                    string date = batchStartDate.Month.ToString(CultureInfo.InvariantCulture) + "-" +
                                  batchStartDate.Year.ToString(CultureInfo.InvariantCulture);
                    Console.WriteLine("\nRoutine started for {0}", date);

                    // Step 3. Check Db and get billing and summary data from API accordingly
                    GetFilteredCspBillingDataFromApi(date, out filteredCspSummaryRecords, out filteredCspBillingRecords);

                    // Get new records for invoices which do not exist in db
                    //// List<CspBillingData> filteredBillingRecords = GetNewerCspBillingRecords(cspBillingRecordsFromApi);

                    if (filteredCspSummaryRecords.Body != null && filteredCspSummaryRecords.Body.Count > 0 &&
                        filteredCspBillingRecords.Body != null && filteredCspBillingRecords.Body.Count > 0)
                    {
                        // Step 4. Parse the raw data to List of CspBillingData and CspSummaryData
                        List<CspSummaryData> parsedSummaryRecords = GetParsedCspSummaryRecords(filteredCspSummaryRecords);
                        List<CspBillingData> parsedBillingRecords = GetParsedCspBillingRecords(filteredCspBillingRecords);

                        // Step 5. Update data in CspBilling and CspSummary data Table
                        int recordsCount = UpdateCspRecordsInDatabase(parsedBillingRecords, parsedSummaryRecords);
                        totalRecordsCount += recordsCount;
                        status = 1;
                        Console.WriteLine("A total of " + recordsCount +
                                          " new records successfully appended to the database tables: CspBillingData, CspSummaryData.");

                        // Step 6.Keep a backup of Billing data in Azure storage
                        if (blobFilename == null)
                        {
                            blobFilename = "CspBillingData_" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" +
                                           DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" +
                                           DateTime.Now.Second + ".csv";
                        }

                        cspAggregateData.AddRange(filteredCspBillingRecords.Body.ToList());
                        Console.WriteLine("Appending data in for Blob storage. It will be uploaded later.");
                        //// blobStorageUri = UpdateCspRecordsInAzureStorage(filteredCspBillingRecords.Body.ToList(), blobFilename);
                    }
                    else
                    {
                        Console.WriteLine("No operation performed on DB and storage. ");
                        status = 3;
                    }

                    batchStartDate = batchStartDate.AddMonths(1);
                }

                if (blobFilename != null && cspAggregateData.Count > 0)
                {
                    blobStorageUri = UpdateCspRecordsInAzureStorage(cspAggregateData, blobFilename);
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

            ; IList < AzureUtilizationRecord> cspUtilizationRecordsFromApi = null;
            Console.WriteLine("\nNow fetching utilization records from the API.");

            // Fetch from API or directly fetch from Partner Center
            // 0 - Fetch from API
            // 1 - Fetch directly

            if (ConfigurationManager.AppSettings["UtilizationAccessSource"] == "0")
            {
                Console.WriteLine("\nNow fetching utilization records from the API.");
                var x = new CspUtilization(AzureAnalyticsApi);
                HttpOperationResponse<IList<AzureUtilizationRecord>> cspUtilizationRecordsFromApiResponse 
                    = x.GetDataForCustomerSubscriptionWithHttpMessagesAsync().Result;
                cspUtilizationRecordsFromApi = cspUtilizationRecordsFromApiResponse.Body;
            }

            else
            {
                Console.WriteLine("\nNow fetching utilization records from the Partner Center SDK.");
                CspUtilizationHelper helperObj = new CspUtilizationHelper();
                cspUtilizationRecordsFromApi = helperObj.doTheTask();
            }
            //string output = JsonConvert.SerializeObject(cspUtilizationRecordsFromApi.Body);
            //System.IO.File.WriteAllText(@".\customJson2.json", output);
            //string json = System.IO.File.ReadAllText(@".\customJson2.json");
            //IList<AzureUtilizationRecord> itemsFromApi = JsonConvert.DeserializeObject<IList<AzureUtilizationRecord>>(cspUtilizationRecordsFromApi.Body);

            if (cspUtilizationRecordsFromApi != null && cspUtilizationRecordsFromApi.Count > 0)
            {
                totalRecordsCount += CspDataHelper.UpdateCurrentUtilizationRecordsInDb(cspUtilizationRecordsFromApi);
            }
            status = 1;

            Console.WriteLine("Successfully wrote data to the Utlization Table");
        }

        internal static int UpdateCurrentUtilizationRecordsInDb(IList<AzureUtilizationRecord> cspUtilizationRecordsFromApi)
        {

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("UsageStartTime", typeof(DateTime));
            dataTable.Columns.Add("UsageEndTime", typeof(DateTime));
            dataTable.Columns.Add("ResourceId", typeof(string));
            dataTable.Columns.Add("ResourceName", typeof(string));
            dataTable.Columns.Add("ResourceCategory", typeof(string));
            dataTable.Columns.Add("ResourceSubCategory", typeof(string));
            dataTable.Columns.Add("Quantity", typeof(float));
            dataTable.Columns.Add("Unit", typeof(string));
            dataTable.Columns.Add("InfoFields", typeof(string));
            dataTable.Columns.Add("InstanceDataResourceUri", typeof(string));
            dataTable.Columns.Add("InstanceDataLocation", typeof(string));
            dataTable.Columns.Add("InstanceDataPartNumber", typeof(string));
            dataTable.Columns.Add("InstanceDataOrderNumber", typeof(string));
            dataTable.Columns.Add("InstanceDatatags", typeof(string));
            dataTable.Columns.Add("Attributes", typeof(string));
            Console.WriteLine("\nAll Usage data which exist in DB will be deleted and replaced by new line items.");
            
            using (AzureAnalyticsDbModel dbContext = new AzureAnalyticsDbModel())
            {
                int NumberOfTtemsFromDatabase = dbContext.CspUtilizationDatas.Count();
                Console.WriteLine("\n" + NumberOfTtemsFromDatabase + " usage records exist in DB and will be deleted.");
                if (NumberOfTtemsFromDatabase > 0)
                {
                    dbContext.Database.ExecuteSqlCommand("delete from CspUtilizationData");
                }


                List<CspUtilizationData> newItemsForDatabase = new List<CspUtilizationData>();

                foreach (var utilizationRecord in cspUtilizationRecordsFromApi)
                {
                    var arg1 = utilizationRecord.InfoFields.Count == 0 || utilizationRecord.InfoFields == null ? "" : String.Join(",", utilizationRecord.InfoFields.ToArray());
                    var arg2 = utilizationRecord.InstanceData.Tags == null || utilizationRecord.InstanceData.Tags.Count == 0 ? "" : String.Join("/,", utilizationRecord.InstanceData.Tags.ToArray());
                    dataTable.Rows.Add(0, (DateTime?)utilizationRecord.UsageStartTime.Value, (DateTime?)utilizationRecord.UsageEndTime.Value, utilizationRecord.Resource.Id, utilizationRecord.Resource.Name, utilizationRecord.Resource.Category, utilizationRecord.Resource.Subcategory, utilizationRecord.Quantity, utilizationRecord.Unit, arg1, utilizationRecord.InstanceData.ResourceUri, utilizationRecord.InstanceData.Location, utilizationRecord.InstanceData.OrderNumber, utilizationRecord.InstanceData.PartNumber, arg2, utilizationRecord.Attributes.Etag);

                }

                Console.WriteLine("\n" + dataTable.Rows.Count + " new usage records will be added to the database.");
                using (var bulkCopy = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["AzureAnalyticsDbModel"].ConnectionString))
                {

                    bulkCopy.ColumnMappings.Add("UsageStartTime", "UsageStartTime");
                    bulkCopy.ColumnMappings.Add("UsageEndTime", "UsageEndTime");
                    bulkCopy.ColumnMappings.Add("ResourceId", "ResourceId");
                    bulkCopy.ColumnMappings.Add("ResourceName", "ResourceName");
                    bulkCopy.ColumnMappings.Add("ResourceCategory", "ResourceCategory");
                    bulkCopy.ColumnMappings.Add("ResourceSubCategory", "ResourceSubCategory");
                    bulkCopy.ColumnMappings.Add("Quantity", "Quantity");
                    bulkCopy.ColumnMappings.Add("Unit", "Unit");
                    bulkCopy.ColumnMappings.Add("InfoFields", "InfoFields");
                    bulkCopy.ColumnMappings.Add("InstanceDataResourceUri", "InstanceDataResourceUri");
                    bulkCopy.ColumnMappings.Add("InstanceDataLocation", "InstanceDataLocation");
                    bulkCopy.ColumnMappings.Add("InstanceDataPartNumber", "InstanceDataPartNumber");
                    bulkCopy.ColumnMappings.Add("InstanceDataOrderNumber", "InstanceDataOrderNumber");
                    bulkCopy.ColumnMappings.Add("InstanceDataTags", "InstanceDataTags");
                    bulkCopy.ColumnMappings.Add("Attributes", "Attributes");

                    bulkCopy.BatchSize = 10000;
                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = "CspUtilizationData";
                    bulkCopy.WriteToServer(dataTable);
                }

                Console.WriteLine("\n" + newItemsForDatabase.Count() + " new usage records will be added to the database.");


            }
            return 0;
        }

        //internal static int UpdateCurrentUtilizationRecordsInDb(IList<AzureUtilizationRecord> cspUtilizationRecordsFromApi)
        //{
        //    Console.WriteLine("\nAll Usage data which exist in DB will be deleted and replaced by new line items.");
        //    int count;
        //    using (AzureAnalyticsDbModel dbContext = new AzureAnalyticsDbModel())
        //    {
        //        dbContext.Configuration.AutoDetectChangesEnabled = false;
        //        dbContext.Configuration.ValidateOnSaveEnabled = false;
        //        List<CspUtilizationData> itemsFromDatabase = dbContext.CspUtilizationDatas.ToList();
        //        Console.WriteLine("\n" + itemsFromDatabase.Count() + " usage records exist in DB and will be deleted.");
        //        if (itemsFromDatabase.Count() > 0)
        //        {
        //            dbContext.CspUtilizationDatas.RemoveRange(itemsFromDatabase);
        //            dbContext.SaveChanges();
        //        }

        //        List<CspUtilizationData> newItemsForDatabase = new List<CspUtilizationData>();

        //        foreach (AzureUtilizationRecord utilizationRecord in cspUtilizationRecordsFromApi.ToList())
        //        {
        //            newItemsForDatabase.Add(
        //                new CspUtilizationData
        //                {
        //                    UsageStartTime = (DateTime?)utilizationRecord.UsageStartTime.Value,
        //                    UsageEndTime = (DateTime?)utilizationRecord.UsageEndTime.Value,
        //                    ResourceId = utilizationRecord.Resource.Id,
        //                    ResourceName = utilizationRecord.Resource.Name,
        //                    ResourceCategory = utilizationRecord.Resource.Category,
        //                    ResourceSubCategory = utilizationRecord.Resource.Subcategory,
        //                    Quantity = utilizationRecord.Quantity,
        //                    Unit = utilizationRecord.Unit,
        //                    infoFields = utilizationRecord.InfoFields.Count == 0 || utilizationRecord.InfoFields == null? null : String.Join(",", utilizationRecord.InfoFields.ToArray()),
        //                    InstanceDataResourceUri = utilizationRecord.InstanceData.ResourceUri,
        //                    InstanceDataLocation = utilizationRecord.InstanceData.Location,
        //                    InstanceDataOrderNumber = utilizationRecord.InstanceData.OrderNumber,
        //                    InstanceDataPartNumber = utilizationRecord.InstanceData.PartNumber,
        //                    InstanceDatatags = utilizationRecord.InstanceData.Tags == null || utilizationRecord.InstanceData.Tags.Count == 0? null : String.Join(",", utilizationRecord.InstanceData.Tags.ToArray()),
        //                    Attributes = utilizationRecord.Attributes.Etag
        //                });
        //        }
        //        Console.WriteLine("\n" + newItemsForDatabase.Count() + " new usage records will be added to the database.");
        //        string output = JsonConvert.SerializeObject(newItemsForDatabase);
        //        System.IO.File.WriteAllText(@".\customJson.json", output);
        //        string json = System.IO.File.ReadAllText(@".\customJson.json");
        //        List<CspUtilizationData> itemsForDatabase = JsonConvert.DeserializeObject<List<CspUtilizationData>>(json);

        //        dbContext.ChangeTracker.DetectChanges();
        //        dbContext.CspUtilizationDatas.AddRange(itemsForDatabase);
        //        count = dbContext.SaveChanges();

        //    }
        //    return count;
        //}
        /// <summary>
        /// For getting the start and end date for fetching data from config file.
        /// </summary>
        /// <param name="batchStartDate">Start date for collection.</param>
        /// <param name="batchEndDate">End date for collection.</param>
        private static void GetDates(out DateTime batchStartDate, out DateTime batchEndDate)
        {
            // Get dates from the config and string and parse as date type objects
            string startDate = ConfigurationManager.AppSettings["CspBillingPeriodStartDate"];
            string endDate = ConfigurationManager.AppSettings["CspBillingPeriodEndDate"];

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
    }
}