using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingWebJob.AzureAnalyticsDb;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BillingWebJob.Helpers
{
    internal class ConfigHelper
    {
        public static void TestConnections()
        {
            if (Convert.ToInt32(ConfigurationManager.AppSettings["ConfigFileCheck"]) == 0)return;
                 
            Console.WriteLine("Testing AzureWebJobsDashboard Dashboard: " + ConfigHelper.TestStorageConnectionString("AzureWebJobsDashboard", "ConnectionString"));
            Console.WriteLine("Testing AzureWebJobsStorage Dashboard: " + ConfigHelper.TestStorageConnectionString("AzureWebJobsStorage", "ConnectionString"));
            Console.WriteLine("Testing StorageConnectionString Dashboard: " + ConfigHelper.TestStorageConnectionString("StorageConnectionString", "AppSetting"));
            Console.WriteLine("Testing AzureAnalyticsDbModel Dashboard: " + ConfigHelper.TestAzureSqlDatabase());
            Console.WriteLine("Testing AzureAnalyticsDbModel Dashboard: " + ConfigHelper.TestAADAuthentication());
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        public static bool TestStorageConnectionString(string connectionElement, string elementType)
        {
            try
            {
                var connectionString = "";
                switch (elementType)
                {
                    case "ConnectionString":
                        connectionString = ConfigurationManager.ConnectionStrings[connectionElement].ToString();
                        break;
                    case "AppSetting":
                        connectionString = ConfigurationManager.AppSettings[connectionElement];
                        break;
                    default:
                        throw new Exception("invalid element");
                }

                var storageAccount = CloudStorageAccount.Parse(connectionString);
                //// Retrieve storage account from connection string.
                //// Create the blob client.
                var blobClient = storageAccount.CreateCloudBlobClient();
                var serviceProperties = blobClient.GetServiceProperties();
                return true;
            }
            catch (Exception e)
            {
                return false;                
            }
        }

        public static bool TestAADAuthentication()
        {
            var azureAnalyticsApi = new BillingWebJob.AzureAnalyticsApi();
            try
            {
                var cspBillingRecordsFromApi =
                    azureAnalyticsApi.CspBilling.GetSingleMonthDataWithOperationResponseAsync("11-2008").Result;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        } 
        public static bool TestAzureSqlDatabase()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["AzureAnalyticsDbModel"].ToString();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    connection.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
