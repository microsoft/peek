// -----------------------------------------------------------------------
// <copyright file="CspUsageController.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>This Api controller class exposes current CSP usage records which have not been invoiced yet..</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using CspHelpers;
    using Microsoft.Store.PartnerCenter.Models;
    using Microsoft.Store.PartnerCenter.Models.Customers;
    using Microsoft.Store.PartnerCenter.Models.Invoices;
    using Models;

    /// <summary>
    /// This Api controller exposes current CSP usage records which have not been invoiced yet.
    /// </summary>
    [RoutePrefix("api/cspusage")]
    public class CspUsageController : ApiController
    {
        /// <summary>
        /// Authentication helper which generates the token for accessing Partner Center APIs. 
        /// </summary>
        private AuthenticationHelper authHelper = new AuthenticationHelper();

        /// <summary>
        /// API which returns current month's usage records.
        /// </summary>
        /// <returns>Current month's usage records.</returns>
        [Route(@"")]
        public List<CspAzureResourceUsageRecord> GetAllData()
        {
            // Authenticate user:
            var partnerOperations = this.authHelper.UserPartnerOperations;

            ////PartnerUsageSummary UsagePage = partnerOperations.UsageSummary.Get();
            SeekBasedResourceCollection<Customer> customersPage = partnerOperations.Customers.Get();
            List<Customer> customers = customersPage.Items.ToList();

            List<CspAzureResourceUsageRecord> usageRecords = new List<CspAzureResourceUsageRecord>();

            foreach (Customer customer in customers)
            {
                try
                {
                    if(customer.RelationshipToPartner.ToString() != "Reseller")
                    {
                        continue;
                    }
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
                }
                catch (Microsoft.Store.PartnerCenter.Exceptions.PartnerException e)
                {
                    throw new Exception("Could not fetch current usage data. See inner exception for details.", e);
                }
            }

            return usageRecords;
        }
    }
}