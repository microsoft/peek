using BillingWebJob.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Store.PartnerCenter;
using Microsoft.Store.PartnerCenter.Extensions;
using Microsoft.Store.PartnerCenter.Models;
using Microsoft.Store.PartnerCenter.Models.Customers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingWebJob.Helpers
{
    class CspUtilizationHelper
    {
        private AuthenticationHelperForPartnerOperation authHelper = new AuthenticationHelperForPartnerOperation();

        public IList<AzureUtilizationRecord> MapUtilizationToCustomClass(List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord> listFromSDK)
        {
            IList<AzureUtilizationRecord> customList = new List<AzureUtilizationRecord>();
            foreach (var objFromSDK in listFromSDK)
            {
                AzureUtilizationRecord objCustom = new AzureUtilizationRecord();
                objCustom.UsageStartTime = objFromSDK.UsageStartTime.LocalDateTime;
                objCustom.UsageEndTime = objFromSDK.UsageEndTime.LocalDateTime;
                objCustom.Unit = objFromSDK.Unit;
                objCustom.Quantity = (double)objFromSDK.Quantity;
                objCustom.InfoFields = objFromSDK.InfoFields;

                AzureInstanceData objCustomInstanceData = new AzureInstanceData();
                AzureResource objCustomResouceData = new AzureResource();
                Models.ResourceAttributes objCustomResouceAttributes = new Models.ResourceAttributes();

                objCustomInstanceData.Location = objFromSDK.InstanceData.Location;
                objCustomInstanceData.OrderNumber = objFromSDK.InstanceData.OrderNumber;
                objCustomInstanceData.PartNumber = objFromSDK.InstanceData.PartNumber;
                objCustomInstanceData.ResourceUri = objFromSDK.InstanceData.ResourceUri.OriginalString;
                objCustomInstanceData.Tags = objFromSDK.InstanceData.Tags;

                objCustom.InstanceData = objCustomInstanceData;

                objCustomResouceData.Category = objFromSDK.Resource.Category;
                objCustomResouceData.Id = objFromSDK.Resource.Id;
                objCustomResouceData.Name = objFromSDK.Resource.Name;
                objCustomResouceData.Subcategory = objFromSDK.Resource.Subcategory;

                objCustom.Resource = objCustomResouceData;

                objCustomResouceAttributes.Etag = objFromSDK.Attributes.Etag;

                objCustom.Attributes = objCustomResouceAttributes;

                customList.Add(objCustom);
            }
            return customList;
        }

        public IList<AzureUtilizationRecord> doTheTask()
        {
            List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord> ResourceUtilizationList = new List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord>();
            var partnerOperations = this.authHelper.AppPartnerOperations;
            //try
            //{
            //    SeekBasedResourceCollection<Customer> customersPage = partnerOperations.Customers.Get();
            //    List<Customer> customers = customersPage.Items.ToList();
            //    Parallel.ForEach(customers, customer =>
            //    {
            //        ResourceUtilizationList.AddRange(GetDataPerCustomer(customer, partnerOperations));
            //    });

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            //string output = JsonConvert.SerializeObject(ResourceUtilizationList);
            //System.IO.File.WriteAllText(@".\ResourceUtilizationList.json", output);
            string json = System.IO.File.ReadAllText(@".\ResourceUtilizationList.json");
            ResourceUtilizationList = JsonConvert.DeserializeObject<List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord>>(json);

            return MapUtilizationToCustomClass(ResourceUtilizationList);
        }
        public static List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord> GetDataPerSubscription(Customer customer, Microsoft.Store.PartnerCenter.Models.Subscriptions.Subscription subscription, IAggregatePartner partnerOperations)
        {
            List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord> ResourceUtilizationList = new List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord>();
            try
            {
                var subscriptionPage = partnerOperations.Customers.ById(customer.Id).Subscriptions.ById(subscription.Id);
                var utilizationRecords = subscriptionPage.Utilization.Azure.Query(
                       DateTimeOffset.Now.AddYears(-6),
                       DateTimeOffset.Now, Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationGranularity.Daily, size: 500);

                var utilizationRecordEnumerator = partnerOperations.Enumerators.Utilization.Azure.Create(utilizationRecords);

                while (utilizationRecordEnumerator.HasValue)
                {
                    foreach (var item in utilizationRecordEnumerator.Current.Items)
                    {
                        ResourceUtilizationList.Add(item);
                    }
                    utilizationRecordEnumerator.Next();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ResourceUtilizationList;
        }

        public static List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord> GetDataPerCustomer(Customer customer, IAggregatePartner partnerOperations)
        {
            List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord> ResourceUtilizationList = new List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord>();
            try
            {
                var subscriptionsPage = partnerOperations.Customers.ById(customer.Id).Subscriptions.Get();
                List<Microsoft.Store.PartnerCenter.Models.Subscriptions.Subscription> subscriptions = subscriptionsPage.Items.ToList();
                Parallel.ForEach(subscriptions, subscription =>
                {
                    ResourceUtilizationList.AddRange(GetDataPerSubscription(customer, subscription, partnerOperations));
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ResourceUtilizationList;
        }
    }

    public class AuthenticationHelperForPartnerOperation
    {
        /// <summary>
        /// A lazy reference to an user based partner operations.
        /// </summary>
        private IAggregatePartner userPartnerOperations = null;

        /// <summary>
        /// A lazy reference to an application based partner operations.
        /// </summary>
        private IAggregatePartner appPartnerOperations = null;

        /// <summary>
        /// Gets a partner operations instance which is application based authenticated.
        /// </summary>
        /// <value>Application Partner Operations object.</value>
        public IAggregatePartner AppPartnerOperations
        {
            get
            {
                if (this.appPartnerOperations == null)
                {

                    string clientId = ConfigurationManager.AppSettings["ApplicationIdAppAuthentication"];
                    string appSecret = ConfigurationManager.AppSettings["ApplicationSecretAppAuthentication"];
                    string aadAppDomain = ConfigurationManager.AppSettings["DomainAppAuthentication"];
                    string aadAuthorityEndpoint = ConfigurationManager.AppSettings["AuthenticationAuthorityEndpoint"];
                    string graphEndpoint = ConfigurationManager.AppSettings["GraphEndpoint"];

                    IPartnerCredentials appCredentials = PartnerCredentials.Instance.GenerateByApplicationCredentials(
                        clientId,
                        appSecret,
                        aadAppDomain,
                        aadAuthorityEndpoint,
                        graphEndpoint);

                    this.appPartnerOperations = PartnerService.Instance.CreatePartnerOperations(appCredentials);
                }

                return this.appPartnerOperations;
            }
        }
    }

}