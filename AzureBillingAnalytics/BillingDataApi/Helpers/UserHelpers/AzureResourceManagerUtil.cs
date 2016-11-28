// -----------------------------------------------------------------------
// <copyright file="AzureResourceManagerUtil.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>Helper class for all interactions with Azure ARM APIs for Direct User scenario.</summary>
// -----------------------------------------------------------------------

namespace BillingDataApi.UserHelpers
{ 
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Helpers;
    using System.Web.Script.Serialization;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Models;
    using Newtonsoft.Json;

    /// <summary>
    /// Helper class for all interactions with Azure ARM APIs for Direct User scenario.
    /// </summary>
    public static class AzureResourceManagerUtil
    {
        /// <summary>
        /// Method to get collection of Organization details which the given auth token is eligible to access.
        /// </summary>
        /// <param name="result">Auth object.</param>
        /// <returns>Collection of organizations.</returns>
        public static Collection<Organization> GetUserOrganizations(AuthenticationResult result)
        {
            Collection<Organization> organizations = new Collection<Organization>();

            // string tenantId = ConfigurationManager.AppSettings["ida:tenantID"];
            try
            {
                // Get a list of Organizations of which the user is a member            
                string requestUrl = string.Format(
                    CultureInfo.InvariantCulture, 
                    "{0}/tenants?api-version={1}",
                    ConfigurationManager.AppSettings["ida:AzureResourceManagerUrl"],
                    ConfigurationManager.AppSettings["ida:AzureResourceManagerAPIVersion"]);

                // Make the GET request
                HttpResponseMessage response = GetResourceResponse(result, requestUrl);

                // Endpoint returns JSON with an array of Tenant Objects
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    var organizationsResult = Json.Decode(responseContent).value;

                    foreach (var organization in organizationsResult)
                    {
                        organizations.Add(new Organization()
                        {
                            Id = organization.tenantId,
                            ////DisplayName = AzureADGraphAPIUtil.GetOrganizationDisplayName(organization.tenantId),
                            objectIdOfCloudSenseServicePrincipal =
                                AzureADGraphAPIUtil.GetObjectIdOfServicePrincipalInOrganization(
                                    organization.tenantId,
                                    ConfigurationManager.AppSettings["ida:ClientID"])
                        });
                    }
                }
            }
            catch
            {
                // intentionally swallowing the exception - this info is optional and should not be breaking the routine if unavailable.
            }

            return organizations;
        }

        /// <summary>
        /// Method to get all subscriptions allowed to be accessed via given auth header.
        /// </summary>
        /// <param name="org">Organization object.</param>
        /// <param name="result">Auth object.</param>
        /// <returns>Collection of subscriptions.</returns>
        public static Collection<Subscription> GetUserSubscriptions(Organization org, AuthenticationResult result)
        {
            Collection<Subscription> subscriptions = new Collection<Subscription>();

            try
            {
                // Get subscriptions to which the user has some kind of access
                string requestUrl = string.Format(
                    CultureInfo.InvariantCulture, 
                    "{0}/subscriptions?api-version={1}",
                    ConfigurationManager.AppSettings["ida:AzureResourceManagerUrl"],
                    ConfigurationManager.AppSettings["ida:AzureResourceManagerAPIVersion"]);

                // Make the GET request
                HttpResponseMessage response = GetResourceResponse(result, requestUrl);

                // Endpoint returns JSON with an array of Subscription Objects
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    var subscriptionsResult = Json.Decode(responseContent).value;

                    foreach (var subscription in subscriptionsResult)
                    {
                        subscriptions.Add(new Subscription()
                        {
                            Id = subscription.subscriptionId,
                            DisplayName = subscription.displayName,
                            OrganizationId = org.Id
                        });
                    }
                }
            }
            catch
            {
                // intentionally swallowing the exception - this info is optional and should not be breaking the routine if unavailable.
            }

            return subscriptions;
        }

        /// <summary>
        /// Method to get the Usage information for the given set of inputs.
        /// </summary>
        /// <param name="subscriptionId">Subscription Id.</param>
        /// <param name="organizationId">Organization Id.</param>
        /// <param name="authres">Authentication object.</param>
        /// <param name="startDate">Start Date of range.</param>
        /// <param name="endDate">End Date of range.</param>
        /// <returns>List of UsageAggregate Objects.</returns>
        public static List<UsageAggregate> GetUsage(
            string subscriptionId, 
            string organizationId,
            AuthenticationResult authres, 
            string startDate, 
            string endDate)
        {
            if (subscriptionId == null)
            {
                throw new ArgumentNullException("subscriptionId");
            }

            List<UsageAggregate> usage = new List<UsageAggregate>();
            List<UsageAggregate> usagetemp = null;
            string usageDetails = string.Empty;

            int cnt = 0;
            JavaScriptSerializer jss = new JavaScriptSerializer();

            jss.RegisterConverters(new JavaScriptConverter[]
            {
                new DynamicJsonConverter()
            });

            string requestUrl =
                string.Format("https://management.azure.com/subscriptions/{0}/providers/Microsoft.Commerce/UsageAggregates?api-version=2015-06-01-preview&reportedstartTime={1}&reportedEndTime={2}", subscriptionId, startDate, endDate);

            while (cnt < 1)
            {
                try
                {
                    // Get subscriptions to which the user has some kind of access for adate range
                    HttpResponseMessage response = GetResourceResponse(authres, requestUrl);

                    // Endpoint returns JSON with an array of Subscription Objects
                    // id subscriptionId displayName state
                    if (response.IsSuccessStatusCode)
                    {
                        usageDetails = response.Content.ReadAsStringAsync().Result;
                        usagetemp = JsonConvert.DeserializeObject<RootObject>(usageDetails).value;
                        dynamic glossaryEntry = jss.Deserialize(usageDetails, typeof(object)) as dynamic;
                        string nl = glossaryEntry.nextLink;
                        requestUrl = nl;
                        usage.AddRange(usagetemp);

                        if (string.IsNullOrEmpty(nl))
                        {
                            cnt = 1;
                        }
                    }
                }
                catch
                {
                    // intentionally swallowing the exception - this info is optional and should not be breaking the routine if unavailable.
                }
            }

            return usage;
        }

        /// <summary>
        /// Method to get the Rate card information for the given set of inputs.
        /// </summary>
        /// <param name="subscriptionId">Subscription ID.</param>
        /// <param name="organizationId">Organization ID.</param>
        /// <param name="ares">Auth object.</param>
        /// <returns>Rate Card object.</returns>
        public static RateCardPayload GetRates(string subscriptionId, string organizationId, AuthenticationResult ares)
        {
            if (subscriptionId == null)
            {
                throw new ArgumentNullException("subscriptionId");
            }

            string rateCard = string.Empty;

            try
            {
                // Aquire Access Token to call Azure Resource Manager
                // Get subscriptions to which the user has some kind of access
                string requestUrl =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "https://management.azure.com/subscriptions/{0}/providers/Microsoft.Commerce/RateCard?api-version=2015-06-01-preview&$filter=OfferDurableId eq '{1}' and Currency eq '{2}' and Locale eq '{3}' and RegionInfo eq '{4}'",
                        subscriptionId, 
                        ConfigurationManager.AppSettings["ida:OfferDurableId"],
                        ConfigurationManager.AppSettings["ida:Currency"], 
                        ConfigurationManager.AppSettings["ida:Locale"],
                        ConfigurationManager.AppSettings["ida:RegionInfo"]);

                // Make the GET request
                HttpResponseMessage response = GetResourceResponse(ares, requestUrl);

                // Endpoint returns JSON with an array of Subscription Objects
                // id subscriptionId displayName state
                if (response.IsSuccessStatusCode)
                {
                    rateCard = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch
            {
            }

            return JsonConvert.DeserializeObject<RateCardPayload>(rateCard);
        }

        /// <summary>
        /// Method to extract the resources associated with with the given list of usage line items.
        /// </summary>
        /// <param name="usageList">Usage line items.</param>
        /// <returns>Resources associated with usage line items.</returns>
        public static IEnumerable<string> GetResources(List<UsageAggregate> usageList)
        {
            IEnumerable<string> resourcesEnumerable = usageList.Select(x => x.properties.meterCategory).Distinct();
            return resourcesEnumerable;
        }

        /// <summary>
        /// Method to calculate the sum across resources.
        /// </summary>
        /// <param name="resourceList">Resources list.</param>
        /// <param name="usageList">Usage line items.</param>
        /// <returns>Resource wise totals.</returns>
        public static IEnumerable<ResourceTotal> GetResourceTotals(
            IEnumerable<string> resourceList,
            List<UsageAggregate> usageList)
        {
            IEnumerable<ResourceTotal> totals =
                from list in usageList
                group list by list.properties.meterCategory
                into prodGroup
                select new ResourceTotal(prodGroup.Key, prodGroup.Sum(p => p.properties.quantity));

            return totals;
        }

        /// <summary>
        /// Method to get resource wise usage totals for given resources.
        /// </summary>
        /// <param name="usageList">Usage Line items.</param>
        /// <param name="resources">Resources for which totals will be calculated.</param>
        /// <returns>Resource usage totals.</returns>
        public static IEnumerable<ResourceUsage> GetResourceUsage(
            List<UsageAggregate> usageList,
            IEnumerable<string> resources)
        {
            IEnumerable<ResourceUsage> totals =
                from resource in resources
                select new ResourceUsage(resource, GetMeterIdTotalsByResource(usageList, resource));
            return totals;
        }

        /// <summary>
        /// Method to get all unique projects associated with the usage line items.
        /// </summary>
        /// <param name="usageList">Usage line items.</param>
        /// <returns>Associated projects.</returns>
        public static IEnumerable<string> GetUniqueProjects(List<UsageAggregate> usageList)
        {
            IEnumerable<string> projects = usageList.Select(a => a.properties.infoFields.Project).Distinct();

            return projects;
        }

        /// <summary>
        /// Method to get Rate Card object.
        /// </summary>
        /// <returns>Rate Card object.</returns>
        public static RateCardPayload GetRateCardPayload()
        {
            RateCardPayload resourceRatesRootObject =
                JsonConvert.DeserializeObject<RateCardPayload>(
                    File.ReadAllText(@"C:\Users\MoinakBandyopadhyay\Desktop\response.json"));
            return resourceRatesRootObject;
        }

        /// <summary>
        /// Method to get Meter Id totals.
        /// </summary>
        /// <param name="usageList">Usage Line items.</param>
        /// <returns>Meter Id totals.</returns>
        public static IEnumerable<MeterIDTotal> GetMeterIdTotals(List<UsageAggregate> usageList)
        {
            var totals =
                from list in usageList
                group list by list.properties.meterId
                into prodGroup
                select new MeterIDTotal(prodGroup.Key, prodGroup.Sum(p => p.properties.quantity));
            return totals;
        }

        /// <summary>
        /// Method to calculate project wise totals.
        /// </summary>
        /// <param name="usageList">Usage line items.</param>
        /// <param name="projects">Projects for which totals will be calculated.</param>
        /// <returns>Project wise usage totals.</returns>
        public static IEnumerable<ProjectUsage> GetProjectIDTotals(
            List<UsageAggregate> usageList,
            IEnumerable<string> projects)
        {
            IEnumerable<ProjectUsage> totals =
                from project in projects
                select new ProjectUsage(project, GetMeterIdTotalsByProjectName(usageList, project));
            return totals;
        }

        /// <summary>
        /// Method to get Meter Id totals by project name.
        /// </summary>
        /// <param name="usageList">Usage line items.</param>
        /// <param name="projectName">Project name.</param>
        /// <returns>Meter id totals.</returns>
        public static IEnumerable<MeterIDTotal> GetMeterIdTotalsByProjectName(
            List<UsageAggregate> usageList,
            string projectName)
        {
            var totals =
                from list in usageList
                where list.properties.infoFields.Project == projectName
                group list by list.properties.meterId
                into prodGroup
                select new MeterIDTotal(prodGroup.Key, prodGroup.Sum(p => p.properties.quantity));
            return totals;
        }

        /// <summary>
        /// Method to get Meter Id totals by resource name.
        /// </summary>
        /// <param name="usageList">Usage line items.</param>
        /// <param name="resourceName">Resource name.</param>
        /// <returns>Meter id totals.</returns>
        public static IEnumerable<MeterIDTotal> GetMeterIdTotalsByResource(
            List<UsageAggregate> usageList,
            string resourceName)
        {
            var totals =
                from list in usageList
                where list.properties.meterCategory == resourceName
                group list by list.properties.meterId
                into prodGroup
                select new MeterIDTotal(prodGroup.Key, prodGroup.Sum(p => p.properties.quantity));
            return totals;
        }

        /// <summary>
        /// Method to calculate the quantity which is ratable.
        /// </summary>
        /// <param name="usage">Total used units.</param>
        /// <param name="includedQuantity">Included units.</param>
        /// <returns>Ratable quantity.</returns>
        public static double GetRatableUsage(double usage, double includedQuantity)
        {
            if ((usage - includedQuantity) > 0)
            {
                return usage - includedQuantity;
            }
            else
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Method to get Meter id rates.
        /// </summary>
        /// <param name="meterIdTotals">Meter id totals.</param>
        /// <param name="rates">Rate card information.</param>
        /// <returns>Rated usage.</returns>
        public static IEnumerable<RatedUsage> GetMeterIDRates(
            IEnumerable<MeterIDTotal> meterIdTotals,
            RateCardPayload rates)
        {
            IEnumerable<RatedUsage> ratesList = from m in meterIdTotals
                select new RatedUsage(
                    m.MeterId,
                    m.ResourceTotals,
                    GetRatesForMeterID(rates, m.MeterId),
                    ComputeRatedUsagePerMeter(GetRatesForMeterID(rates, m.MeterId), GetRatableUsage(m.ResourceTotals, GetIncludedQuantityForMeterID(rates, m.MeterId))),
                    GetMeterSubCategoryByMeterId(rates, m.MeterId),
                    GetServiceByMeterId(rates, m.MeterId),
                    GetResourceNameByMeterId(rates, m.MeterId));

            return ratesList;
        }

        /// <summary>
        /// Method to get project rates.
        /// </summary>
        /// <param name="rates">Rate card information.</param>
        /// <param name="projects">Projects collection.</param>
        /// <returns>Project rates.</returns>
        public static IEnumerable<ProjectRate> GetProjectRates(RateCardPayload rates, IEnumerable<ProjectUsage> projects)
        {
            IEnumerable<ProjectRate> ratesList = from p in projects
                select new ProjectRate(
                    p.ProjectName, GetMeterIDRates(p.MeterIdTotals, rates));

            return ratesList;
        }

        /// <summary>
        /// Method to get Resource rates.
        /// </summary>
        /// <param name="rates">Rate card information.</param>
        /// <param name="resourceUsage">Resource usage collection.</param>
        /// <returns>Resource rate collection.</returns>
        public static IEnumerable<ResourceRate> GetResourceRates(
            RateCardPayload rates,
            IEnumerable<ResourceUsage> resourceUsage)
        {
            IEnumerable<ResourceRate> ratesList = from r in resourceUsage
                select new ResourceRate(
                    r.ResourceName, GetMeterIDRates(r.MeterIdTotals, rates));

            return ratesList;
        }

        /// <summary>
        /// Method to compute rated usage per meter.
        /// </summary>
        /// <param name="rates">Rates information.</param>
        /// <param name="usage">Usage information.</param>
        /// <returns>Total value of rated usage.</returns>
        public static double ComputeRatedUsagePerMeter(Dictionary<double, double> rates, double usage)
        {
            double total = 0.0;

            if (rates.Count == 0)
            {
                return 0.0;
            }
            else if (rates.Count == 1)
            {
                return usage * rates.Values.FirstOrDefault();
            }

            var remainingUsage = usage;

            while (rates.Count > 0)
            {
                ////double currentValue=rates.GetEnumerator().Current.Key;
                double lastKey = rates.Keys.Last();

                if (remainingUsage > lastKey)
                {
                    double lastKeyValue = 0.0;
                    if (rates.TryGetValue(lastKey, out lastKeyValue))
                    {
                        total = total + ((remainingUsage - lastKey + 1) * lastKeyValue);
                        remainingUsage = lastKey - 1;
                    }

                    rates.Remove(lastKey);
                }
                else if (remainingUsage <= lastKey)
                {
                    rates.Remove(lastKey);
                }
            }

            return total;
        }

        /// <summary>
        /// Method to get rates for Meter ID.
        /// </summary>
        /// <param name="rates">Rates information.</param>
        /// <param name="meterId">Meter id.</param>
        /// <returns>Meter rates.</returns>
        public static Dictionary<double, double> GetRatesForMeterID(RateCardPayload rates, string meterId)
        {
            List<Resource> resources = rates.Meters;
            Resource r = resources.Find(x => x.MeterId == meterId);
            if (r == null)
            {
                r = GetUndefinedResource();
            }

            return r.MeterRates;
        }

        /// <summary>
        /// Method to get meter sub-category by Meter Id
        /// </summary>
        /// <param name="rates">rates information</param>
        /// <param name="meterId">meter id</param>
        /// <returns>Meter sub-category</returns>
        public static string GetMeterSubCategoryByMeterId(RateCardPayload rates, string meterId)
        {
            List<Resource> resources = rates.Meters;

            IEnumerable<string> rate = from r in resources
                where r.MeterId == meterId
                select r.MeterSubCategory;
            return rate.FirstOrDefault();
        }

        /// <summary>
        /// method to get Service by Meter Id
        /// </summary>
        /// <param name="rates">rates information</param>
        /// <param name="meterId">Meter id</param>
        /// <returns>Service name</returns>
        public static string GetServiceByMeterId(RateCardPayload rates, string meterId)
        {
            List<Resource> resources = rates.Meters;

            IEnumerable<string> rate = from r in resources
                where r.MeterId == meterId
                select r.MeterCategory;

            return rate.FirstOrDefault();
        }

        /// <summary>
        /// method to get Resource Name by Meter Id
        /// </summary>
        /// <param name="rates">rates information</param>
        /// <param name="meterId">Meter id</param>
        /// <returns>Resource name</returns>
        public static string GetResourceNameByMeterId(RateCardPayload rates, string meterId)
        {
            List<Resource> resources = rates.Meters;

            IEnumerable<string> rate = from r in resources
                where r.MeterId == meterId
                select r.MeterName;

            return rate.FirstOrDefault();
        }

        /// <summary>
        /// Method to get included quantity for meter Id
        /// </summary>
        /// <param name="rates">rates information</param>
        /// <param name="meterId">meter id</param>
        /// <returns>included quantity</returns>
        public static double GetIncludedQuantityForMeterID(RateCardPayload rates, string meterId)
        {
            List<Resource> resources = rates.Meters;

            IEnumerable<double> includedquantity = from r in resources
                where r.MeterId == meterId
                select r.IncludedQuantity;

            return includedquantity.FirstOrDefault();
        }

        /// <summary>
        /// method to get Service Totals
        /// </summary>
        /// <param name="usageList">Usage line items</param>
        /// <returns>Service Totals</returns>
        public static IEnumerable<ServiceTotal> GetServiceTotals(List<UsageAggregate> usageList)
        {
            IEnumerable<ServiceTotal> totals =
                from list in usageList
                group list by list.properties.meterName
                into prodGroup
                select new ServiceTotal(prodGroup.Key, prodGroup.Sum(p => p.properties.quantity));

            return totals;
        }

        /// <summary>
        /// Method to get rated estimate
        /// </summary>
        /// <param name="ratedUsageList">Rated Usage line items</param>
        /// <returns>rated estimate</returns>
        public static double RatedEstimate(IEnumerable<RatedUsage> ratedUsageList)
        {
            double ratedUsageEstimate = ratedUsageList.Sum(c => c.RatedTotal);
            return ratedUsageEstimate;
        }

        /// <summary>
        /// Method to rated estimate for project
        /// </summary>
        /// <param name="projectRateList">Project rate list</param>
        /// <returns>rated estimate for project</returns>
        public static IEnumerable<ProjectEstimate> RatedEstimateForProject(IEnumerable<ProjectRate> projectRateList)
        {
            IEnumerable<ProjectEstimate> projectEstimates = from p in projectRateList
                select new ProjectEstimate(p.ProjectName, RatedEstimate(p.Rates));
            return projectEstimates;
        }

        /// <summary>
        /// Method to rated estimate for resource
        /// </summary>
        /// <param name="resourceRateList">resource rate list</param>
        /// <returns>rated estimate for resource</returns>
        public static IEnumerable<ResourceEstimate> RatedEstimateForResource(IEnumerable<ResourceRate> resourceRateList)
        {
            IEnumerable<ResourceEstimate> projectEstimates = from r in resourceRateList
                select new ResourceEstimate(r.ResourceName, RatedEstimate(r.Rates));
            return projectEstimates;
        }

        /// <summary>
        /// Method to get undefined resource
        /// </summary>
        /// <returns>get undefined resource</returns>
        public static Resource GetUndefinedResource()
        {
            Resource r = new Resource()
            {
                EffectiveDate = "0001-01-01T00:00:00",
                IncludedQuantity = 0,
                MeterCategory = "Unknown Category",
                MeterId = "00000000-000-0000-0000-000000000000",
                MeterName = "Unknown Service",
                MeterRates = new Dictionary<double, double>(),
                MeterSubCategory = string.Empty,
                Unit = "No Unit"
            };
            return r;
        }

        /// <summary>
        /// Method to add auth header, send the request and return the API response.
        /// </summary>
        /// <param name="result">auth token container object.</param>
        /// <param name="requestUrl">request Url</param>
        /// <returns>Response object.</returns>
        private static HttpResponseMessage GetResourceResponse(AuthenticationResult result, string requestUrl)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            HttpResponseMessage response = client.SendAsync(request).Result;
            return response;
        }
    }
}