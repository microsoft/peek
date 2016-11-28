namespace BillingDataApi.Models
{
    public class MeterIDTotal
    {
        public string MeterId { get; private set; }

        //public string Service { get; private set; }
        public double ResourceTotals { get; private set; }

        //public string Service { get; set; }
        public MeterIDTotal(string meterId, double resourceTotals)
        {
            this.MeterId = meterId;
            this.ResourceTotals = resourceTotals;
            //this.Service = service;
        }
    }
}