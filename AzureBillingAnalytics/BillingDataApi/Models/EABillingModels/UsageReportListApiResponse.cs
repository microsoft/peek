using System.Runtime.Serialization;

namespace BillingDataApi.Models.EABillingModels
{
    /// <summary>
    /// the response data contract for GetUsageList.
    /// </summary>
    [DataContract]
    public class UsageReportListApiResponse
    {
        public UsageReportListApiResponse()
        {
            this.ObjectType = "Usage";
        }

        [DataMember]
        public UsageMonth[] AvailableMonths { get; set; }

        [DataMember]
        public string ObjectType { get; set; }
    }

    [DataContract]
    public class UsageMonth
    {
        [DataMember]
        public string Month { get; set; }

        [DataMember]
        public string LinkToDownloadSummaryReport { get; set; }

        [DataMember]
        public string LinkToDownloadDetailReport { get; set; }
    }
}