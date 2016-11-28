namespace BillingDataApi.Models
{
    public class UsageOutput
    {
        public int Id { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string SubscriptionId { get; set; }
        public string ProjectName { get; set; }
        public double ProjectEstimate { get; set; }
        public double MeterIdTotals { get; set; }
        public string UsageStartTime { get; set; }
        public string UsageEndTime { get; set; }
        public string MeterId { get; set; }
        public string Unit { get; set; }
        public string MeterName { get; set; }
        public string MeterCategory { get; set; }
        public string MeterSubCategory { get; set; }
        public string MeterRegion { get; set; }
        public double Quantity { get; set; }
        public string MeteredService { get; set; }
        public string MeteredServiceType { get; set; }
        public string ServiceInfo { get; set; }
    }
}