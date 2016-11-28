using System.Collections.Generic;

namespace BillingDataApi.Models
{
    public class HomeIndexViewModel
    {
        public Dictionary<string, Organization> UserOrganizations { get; set; }
        public Dictionary<string, Subscription> UserSubscriptions { get; set; }
        public string UsageString { get; set; }
    }
}