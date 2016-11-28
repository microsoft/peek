using System.Collections.Generic;
using Newtonsoft.Json;

namespace BillingDataApi.Models
{
    public class Properties
    {
        public string subscriptionId { get; set; }
        public string usageStartTime { get; set; }
        public string usageEndTime { get; set; }
        public string meterId { get; set; }
        public InfoFields infoFields { get; set; }

        [JsonProperty("instanceData")]
        public string instanceDataRaw { get; set; }

        public InstanceDataType InstanceData
        {
            get { return JsonConvert.DeserializeObject<InstanceDataType>(instanceDataRaw.Replace("\\\"", "")); }
        }

        public double quantity { get; set; }
        public string unit { get; set; }
        public string meterName { get; set; }
        public string meterCategory { get; set; }
        public string meterSubCategory { get; set; }
        public string meterRegion { get; set; }
    }

    public class InstanceDataType
    {
        [JsonProperty("Microsoft.Resources")]
        public MicrosoftResourcesDataType MicrosoftResources { get; set; }
    }

    public class MicrosoftResourcesDataType
    {
        public string resourceUri { get; set; }

        public IDictionary<string, string> tags { get; set; }

        public IDictionary<string, string> additionalInfo { get; set; }

        public string location { get; set; }

        public string partNumber { get; set; }

        public string orderNumber { get; set; }
    }
}