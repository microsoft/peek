namespace BillingDataApi.Models
{
    public class ResourceEstimate
    {
        public string ResourceName { get; set; }
        public double Estimate { get; set; }

        public ResourceEstimate(string resourcename, double estimate)
        {
            this.ResourceName = resourcename;
            this.Estimate = estimate;
        }
    }
}