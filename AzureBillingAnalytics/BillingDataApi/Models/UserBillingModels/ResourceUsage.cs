using System.Collections.Generic;

namespace BillingDataApi.Models
{
    public class ResourceUsage
    {
        public string ResourceName { get; set; }

        public IEnumerable<MeterIDTotal> MeterIdTotals { get; set; }

        public ResourceUsage(string resourcename, IEnumerable<MeterIDTotal> meteridtotals)
        {
            this.ResourceName = resourcename;
            this.MeterIdTotals = meteridtotals;
        }
    }
}