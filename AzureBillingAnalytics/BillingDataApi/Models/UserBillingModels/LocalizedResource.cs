namespace BillingDataApi.Models
{
    public class LocalizedResource
    {
        public string MeterName { get; set; }
        public string MeterCategory { get; set; }
        public string MeterSubCategory { get; set; }
        public string Unit { get; set; }
        public string CultureInfo { get; set; }
    }
}