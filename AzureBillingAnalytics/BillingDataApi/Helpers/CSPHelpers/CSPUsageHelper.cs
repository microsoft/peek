using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BillingDataApi.CspHelpers;
using Microsoft.Store.PartnerCenter.Models;
using Microsoft.Store.PartnerCenter.Models.Customers;
using Microsoft.Store.PartnerCenter.Models.Invoices;
using BillingDataApi.Models;
using Microsoft.Store.PartnerCenter;

namespace BillingDataApi.Helpers.CSPHelpers
{
    public class CSPUsageHelper
    {
        
        public static List<CspAzureResourceUsageRecord> GetRecordsFromCustomer(List<Customer> customers, IAggregatePartner partnerOperations)
        {
            List<CspAzureResourceUsageRecord> usageRecords = new List<CspAzureResourceUsageRecord>();
            // async for each
            Parallel.ForEach(customers, customer => {
                usageRecords.AddRange(CSPUsageHelper.GetDataForCustomer(customer, partnerOperations));
            });
            return usageRecords;
        }
        public static List<CspAzureResourceUsageRecord> GetDataForCustomer(Customer customer, IAggregatePartner partnerOperations)
        {
            List<CspAzureResourceUsageRecord> usageRecords = new List<CspAzureResourceUsageRecord>();
            var subscriptionsPage = partnerOperations.Customers.ById(customer.Id).Subscriptions.Get();

            List<Microsoft.Store.PartnerCenter.Models.Subscriptions.Subscription> currentSubscriptions =
                subscriptionsPage.Items.ToList();

            foreach (
                Microsoft.Store.PartnerCenter.Models.Subscriptions.Subscription subscription in
                currentSubscriptions)
            {
                if (subscription.BillingType != BillingType.Usage)
                {
                    continue;
                }

                var subscriptionPage =
                    partnerOperations.Customers.ById(customer.Id).Subscriptions.ById(subscription.Id);

                var subscriptionUsageRecords = subscriptionPage.UsageRecords.Resources.Get();

                if (subscriptionUsageRecords.TotalCount > 0)
                {
                    var subscriptionUsageSummary = subscriptionPage.UsageSummary.Get();
                    foreach (var item in subscriptionUsageRecords.Items.ToList())
                    {
                        CspAzureResourceUsageRecord csprec = new CspAzureResourceUsageRecord();

                        csprec.Category = item.Category;
                        csprec.QuantityUsed = item.QuantityUsed;
                        csprec.ResourceId = item.ResourceId;
                        csprec.ResourceName = item.ResourceName;
                        csprec.Subcategory = item.Subcategory;
                        csprec.TotalCost = item.TotalCost;
                        csprec.Unit = item.Unit;

                        // Customer profile info:
                        csprec.CustomerId = customer.Id;
                        csprec.CustomerName = customer.CompanyProfile.CompanyName;
                        ////csprec.CustomerBillingProfile = customer.BillingProfile.ToString();
                        csprec.CustomerCommerceId = customer.CommerceId;
                        csprec.CustomerDomain = customer.CompanyProfile.Domain;
                        csprec.CustomerTenantId = customer.CompanyProfile.TenantId;
                        csprec.CustomerRelationshipToPartner = customer.RelationshipToPartner.ToString();

                        // Subscription profile info:
                        csprec.SubscriptionId = subscription.Id;
                        csprec.SubscriptionName = subscription.FriendlyName;
                        csprec.SubscriptionStatus = subscription.Status.ToString();
                        csprec.SubscriptionContractType = subscription.ContractType.ToString();

                        // Billing cycle info:
                        csprec.BillingStartDate = subscriptionUsageSummary.BillingStartDate.DateTime;
                        csprec.BillingEndDate = subscriptionUsageSummary.BillingEndDate.DateTime;
                        usageRecords.Add(csprec);
                    }
                }
            }
            return usageRecords;
        }
            
    }

}