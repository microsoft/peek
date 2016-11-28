namespace BillingWebJob.AzureAnalyticsDb
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AuditData")]
    public partial class AuditData
    {
        public int ID { get; set; }

        public DateTime? TimeStamp { get; set; }

        public int? RecordCount { get; set; }

        public string BlobStorageUrl { get; set; }

        public int? Status { get; set; }

        public string ErrorMessage { get; set; }

        [StringLength(50)]
        public string CustomerType { get; set; }
    }
}