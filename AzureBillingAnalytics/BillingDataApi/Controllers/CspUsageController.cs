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
    using System.Diagnostics;
    using System.Web.Script.Serialization;
    using System.Web;
    using Helpers.CSPHelpers;

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
            usageRecords = CSPUsageHelper.GetRecordsFromCustomer(customers, partnerOperations);
            return usageRecords;
        }        
    }
}