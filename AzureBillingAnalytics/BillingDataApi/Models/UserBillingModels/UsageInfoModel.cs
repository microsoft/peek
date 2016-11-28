namespace BillingDataApi.Models
{
    public class UsageInfoModel
    {
        public int UsageInfoModelId { get; set; }
        public string SubceriptionId { get; set; }
        public string OrganizationId { get; set; }
        public string MeterName { get; set; }
        public string MeterCategory { get; set; }
        public string MeteredRegion { get; set; }
        public string MeterSubCategory { get; set; }
        public string UsageStartTime { get; set; }
        public string UsageEndTime { get; set; }
        public string MeteredService { get; set; }
        public string MeteredServiceType { get; set; }
        public string UserProject { get; set; }
        public double Quantity { get; set; }
        public double ItemTotal { get; set; }
    }
}