using System.Collections.Generic;

namespace BillingDataApi.Models
{
    public class ResourceRate
    {
        public string ResourceName { get; set; }

        public IEnumerable<RatedUsage> Rates { get; set; }

        public ResourceRate(string resourceName, IEnumerable<RatedUsage> rates)
        {
            this.ResourceName = resourceName;
            this.Rates = rates;
        }
    }
}