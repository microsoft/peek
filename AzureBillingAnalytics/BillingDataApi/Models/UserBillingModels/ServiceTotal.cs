namespace BillingDataApi.Models
{
    public class ServiceTotal
    {
        public string MeterId { get; private set; }

        public double ResourceTotals { get; private set; }

        public ServiceTotal(string meterId, double resourceTotals)
        {
            this.MeterId = meterId;
            this.ResourceTotals = resourceTotals;
        }
    }
}