// -----------------------------------------------------------------------
// <copyright file="UserBillingController.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>This api controller class exposes usage data for direct subscriptions.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Globalization;
    using System.Web.Http;
    using Helpers.ParameterValidators;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Models;
    using Newtonsoft.Json;
    using UserHelpers;

    /// <summary>
    /// This api controller exposes usage data for direct subscriptions.
    /// </summary>
    [RoutePrefix("api/userbilling")]
    public class UserBillingController : ApiController
    {
        /// <summary>
        /// Authentication helper which generates the token for accessing azure usage APIs. 
        /// </summary>
        private static AuthenticationResult authResult = AuthResult.GetAuthenticationResult();
        ////private static Collection<Organization> organizations = AzureResourceManagerUtil.GetUserOrganizations(ares);

        /// <summary>
        /// Tenant id as specified in the configurations.
        /// </summary>
        private static string orgId = ConfigurationManager.AppSettings["ida:tenantID"];

        /// <summary>
        /// Organization details fetched for the given Tenant id.
        /// </summary>
        private static Organization org = new Organization
        {
            Id = orgId,
            DisplayName = AzureADGraphAPIUtil.GetOrganizationDisplayName(orgId),
            objectIdOfCloudSenseServicePrincipal =
                AzureADGraphAPIUtil.GetObjectIdOfServicePrincipalInOrganization(
                    orgId,
                    ConfigurationManager.AppSettings["ida:ClientID"])
        };

        /// <summary>
        /// Collection of all records to be collected for the given tenant id.
        /// </summary>
        private List<UsageInfoModel> usageInfo = new List<UsageInfoModel>();

        /// <summary>
        /// End date as string for which the date range for which data will be collected.
        /// </summary>
        private string endDate = string.Empty;

        /// <summary>
        /// Start date as string for which the date range for which data will be collected.
        /// </summary>
        private string startDate = string.Empty;

        /// <summary>
        /// All subscriptions associated with the given tenant id.
        /// </summary>
        private Collection<Subscription> subscriptions = AzureResourceManagerUtil.GetUserSubscriptions(org, authResult);

        /// <summary>
        /// Api which returns usage records for an azure subscription for the current month.
        /// </summary>
        /// <returns>Collection of Usage records.</returns>
        [Route(@"")]
        public IEnumerable<UsageInfoModel> GetCurrentMonthData()
        {
            // return data for current month from 1st to today's date:
            DateTime stdate = DateTime.Now;
            this.endDate = stdate.ToString("o", CultureInfo.InvariantCulture).Substring(0, 10) + "+00%3a00%3a00Z";
            this.startDate = stdate.AddDays(-stdate.Day + 1).ToString("o", CultureInfo.InvariantCulture).Substring(0, 10) +
                        "+00%3a00%3a00Z";

            return this.GetUsageInfo();
        }

        /// <summary>
        /// Api which returns usage records for an azure subscription for the given month.
        /// </summary>
        /// <param name="monthYear">Provide month and year in "mm-yyyy" format. Example: "02-2016".</param>
        /// <returns>Collection of Usage records.</returns>
        [Route(@"bymonth/{monthYear}")]
        public IEnumerable<UsageInfoModel> GetSingleMonthData([FromUri] string monthYear)
        {
            UrlParameterValidation.ValidateMonthYearFormat(monthYear);
            int month = int.Parse(monthYear.Split('-')[0], CultureInfo.InvariantCulture);
            int year = int.Parse(monthYear.Split('-')[1], CultureInfo.InvariantCulture);

            DateTime stdate = new DateTime(year, month, 1);
            DateTime eddate = stdate.AddMonths(1);

            this.startDate = stdate.ToString("o", CultureInfo.InvariantCulture).Substring(0, 10) + "+00%3a00%3a00Z";
            this.endDate = eddate.ToString("o", CultureInfo.InvariantCulture).Substring(0, 10) + "+00%3a00%3a00Z";

            return this.GetUsageInfo();
        }

        /// <summary>
        /// Api which returns usage records for an azure subscription for the given month range.
        /// </summary>
        /// <param name="startMonthYear">Provide month and year in "yyyymm" format. Example: "022015".</param>
        /// <param name="endMonthYear">Provide month and year in "yyyymm" format. Example: "122016".</param>
        /// <returns>Collection of Usage records.</returns>
        [Route(@"bymonthrange/{startMonthYear}/{endMonthYear}")]
        public IEnumerable<UsageInfoModel> GetDataForMonthRange([FromUri] string startMonthYear, [FromUri] string endMonthYear)
        {
            if (string.IsNullOrEmpty(startMonthYear))
            {
                throw new ArgumentNullException(
                    nameof(startMonthYear),
                    "No value provided for parameter. The acceptable value should be in 'yyyymm' format.");
            }

            if (string.IsNullOrEmpty(endMonthYear))
            {
                throw new ArgumentNullException(
                    nameof(endMonthYear),
                    "No value provided for parameter. The acceptable value should be in 'yyyymm' format.");
            }

            if (startMonthYear.Length != 6 || endMonthYear.Length != 6)
            {
                throw new ArgumentException(
                    "Invalid value provided for parameter. The acceptable value should be in yyyymm format.");
            }

            try
            {
                int.Parse(startMonthYear, CultureInfo.InvariantCulture);
                int.Parse(endMonthYear, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    "Invalid value provided for parameter. The acceptable value should be in yyyymm format.", ex);
            }

            startMonthYear = startMonthYear.Substring(0, 3) + "01" + startMonthYear.Substring(2);
            endMonthYear = endMonthYear.Substring(0, 3) + "01" + endMonthYear.Substring(2);
            this.startDate = startMonthYear + "+00%3a00%3a00Z";
            this.endDate = endMonthYear + "+00%3a00%3a00Z";

            return this.GetUsageInfo();
        }

        /// <summary>
        /// Method to collect azure usage records from Azure APIs.
        /// </summary>
        /// <returns>Collection of Usage records.</returns>
        private IEnumerable<UsageInfoModel> GetUsageInfo()
        {
            // Loop for every subscription
            foreach (var subscription in this.subscriptions)
            {
                // Get usage details for subscription
                RateCardPayload rates = AzureResourceManagerUtil.GetRates(subscription.Id, org.Id, authResult);

                List<UsageAggregate> usageList = AzureResourceManagerUtil.GetUsage(
                    subscription.Id,
                    org.Id,
                    authResult,
                    this.startDate,
                    this.endDate);

                IEnumerable<string> projects = AzureResourceManagerUtil.GetUniqueProjects(usageList);
                subscription.Projects = projects;

                IEnumerable<ProjectUsage> projectTotals =
                    AzureResourceManagerUtil.GetProjectIDTotals(usageList, projects);
                subscription.ProjectTotals = projectTotals;

                IEnumerable<ProjectRate> projectRates = AzureResourceManagerUtil.GetProjectRates(
                    rates,
                    projectTotals);
                subscription.ProjectRates = projectRates;

                IEnumerable<ProjectEstimate> projectEstimateTotals =
                    AzureResourceManagerUtil.RatedEstimateForProject(projectRates);
                subscription.ProjectEstimateTotals = projectEstimateTotals;

                IEnumerable<string> resourceList = AzureResourceManagerUtil.GetResources(usageList);

                IEnumerable<ResourceUsage> resourceUsages =
                    AzureResourceManagerUtil.GetResourceUsage(usageList, resourceList);
                subscription.ResourceTotals = AzureResourceManagerUtil.GetResourceTotals(resourceList, usageList);

                IEnumerable<ResourceRate> resourceRates =
                    AzureResourceManagerUtil.GetResourceRates(rates, resourceUsages);

                IEnumerable<ResourceEstimate> resourceEstimates =
                    AzureResourceManagerUtil.RatedEstimateForResource(resourceRates);
                subscription.ResourceEstimates = resourceEstimates;

                subscription.MeterIdTotals = AzureResourceManagerUtil.GetMeterIdTotals(usageList);
                subscription.ServiceTotals = AzureResourceManagerUtil.GetServiceTotals(usageList);

                var result = JsonConvert.SerializeObject(subscription.MeterIdTotals); // Input to graphing solution
                subscription.UsageDetails = result;

                IEnumerable<RatedUsage> ratedUsageList =
                    AzureResourceManagerUtil.GetMeterIDRates(
                        AzureResourceManagerUtil.GetMeterIdTotals(usageList), rates);
                subscription.ratedUsage = ratedUsageList;
                subscription.RatedEstimate = AzureResourceManagerUtil.RatedEstimate(ratedUsageList);

                foreach (var usg in usageList)
                {
                    double usgtot =
                        AzureResourceManagerUtil.ComputeRatedUsagePerMeter(
                            AzureResourceManagerUtil.GetRatesForMeterID(rates, usg.properties.meterId),
                            usg.properties.quantity);

                    this.usageInfo.Add(new UsageInfoModel()
                    {
                        OrganizationId = org.Id,
                        SubceriptionId = subscription.Id,
                        UsageStartTime = usg.properties.usageStartTime,
                        UsageEndTime = usg.properties.usageEndTime,
                        MeteredRegion = usg.properties.infoFields.MeteredRegion,
                        MeterName = usg.properties.meterName,
                        MeteredService = usg.properties.infoFields.MeteredService,
                        MeteredServiceType = usg.properties.infoFields.MeteredServiceType,
                        MeterCategory = usg.properties.meterCategory,
                        MeterSubCategory = usg.properties.meterSubCategory,
                        UserProject = usg.properties.infoFields.Project,
                        Quantity = usg.properties.quantity,
                        ItemTotal = usgtot
                    });
                }
            }

            return this.usageInfo;
        }
    }
}