using System.Collections.Generic;

namespace BillingDataApi.Models
{
    public class RatedUsage
    {
        public string MeterId { get; set; }

        public double Usage { get; set; }

        public Dictionary<double, double> Rate { get; set; }
        public double RatedTotal { get; set; }

        public string ServiceType { get; set; }

        public string Service { get; set; }

        public string ResourceName { get; set; }


        public RatedUsage(string meterid, double usage, Dictionary<double, double> rate, double ratedTotal,
            string servicetype, string service, string resourcename)
        {
            this.MeterId = meterid;
            this.RatedTotal = ratedTotal;
            this.ServiceType = servicetype;
            this.Service = service;
            this.ResourceName = resourcename;
            this.Usage = usage;
            this.Rate = rate;
        }
    }
}