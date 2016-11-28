namespace BillingDataApi.Models
{
    public class ResourceTotal
    {
        public string Resource { get; set; }
        public double Total { get; set; }

        public string MeterId { get; set; }

        public ResourceTotal(string resource, double total)
        {
            this.Resource = resource;
            this.Total = total;
        }
    }
}