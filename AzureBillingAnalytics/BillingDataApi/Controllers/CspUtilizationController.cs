using BillingDataApi.CspHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BillingDataApi.Controllers
{
    [RoutePrefix("api/csputilization")]
    public class CspUtilizationController : ApiController
    {
        private AuthenticationHelper authHelper = new AuthenticationHelper();
        [Route(@"")]
        public List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord> GetDataForCustomerSubscription(string customerId, string subscriptionId)
        {
            var partnerOperations = this.authHelper.UserPartnerOperations;

            var utilizationRecords = partnerOperations.Customers[customerId].Subscriptions[subscriptionId].Utilization.Azure.Query(
                DateTimeOffset.Now.AddYears(-6),
                DateTimeOffset.Now, Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationGranularity.Daily, size:500);

            var utilizationRecordEnumerator = partnerOperations.Enumerators.Utilization.Azure.Create(utilizationRecords);
            List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord> ResourceUtilizationList = new List<Microsoft.Store.PartnerCenter.Models.Utilizations.AzureUtilizationRecord>();
            while (utilizationRecordEnumerator.HasValue)
            {
                foreach (var item in utilizationRecordEnumerator.Current.Items)
                {
                    ResourceUtilizationList.Add(item);                
                }
                utilizationRecordEnumerator.Next();
            }

            return ResourceUtilizationList;
        }
    }
}
