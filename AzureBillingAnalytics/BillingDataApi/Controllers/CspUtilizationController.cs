using BillingDataApi.CspHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Store.PartnerCenter.Models;
using Microsoft.Store.PartnerCenter.Models.Customers;
using System.Diagnostics;
using Microsoft.Store.PartnerCenter;

namespace BillingDataApi.Controllers
{
    [RoutePrefix("api/csputilization")]
    public class CspUtilizationController : ApiController
    {
        private AuthenticationHelper authHelper = new AuthenticationHelper();

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
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
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
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return ResourceUtilizationList;
        }

        [Route(@"")]
        public List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord> GetDataForCustomerSubscription()
        {
            var partnerOperations = this.authHelper.UserPartnerOperations;
            List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord> ResourceUtilizationList = new List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord>();
            try
            {
                SeekBasedResourceCollection<Customer> customersPage = partnerOperations.Customers.Get();
                List<Customer> customers = customersPage.Items.ToList();           
                Parallel.ForEach(customers, customer => {
                    ResourceUtilizationList.AddRange(GetDataPerCustomer(customer, partnerOperations));
                });
                      
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return ResourceUtilizationList;
        }
    }
}
