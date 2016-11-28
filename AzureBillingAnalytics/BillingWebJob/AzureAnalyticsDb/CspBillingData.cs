namespace BillingWebJob.AzureAnalyticsDb
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CspBillingData")]
    public partial class CspBillingData
    {
        public int Id { get; set; }

        public DateTime? UsageDate { get; set; }

        [StringLength(50)]
        public string CustomerBillableAccount { get; set; }

        [StringLength(50)]
        public string PartnerId { get; set; }

        [StringLength(1024)]
        public string PartnerName { get; set; }

        [StringLength(50)]
        public string PartnerBillableAccountId { get; set; }

        [StringLength(1024)]
        public string CustomerCompanyName { get; set; }

        [StringLength(50)]
        public string MpnId { get; set; }

        [Required]
        [StringLength(50)]
        public string TiermpnId { get; set; }

        [StringLength(50)]
        public string InvoiceNumber { get; set; }

        [StringLength(50)]
        public string SubscriptionId { get; set; }

        [StringLength(1024)]
        public string SubscriptionName { get; set; }

        public string SubscriptionDescription { get; set; }

        [StringLength(50)]
        public string OrderId { get; set; }

        [StringLength(255)]
        public string ServiceName { get; set; }

        [StringLength(255)]
        public string ServiceType { get; set; }

        [StringLength(50)]
        public string ResourceGuid { get; set; }

        [StringLength(1024)]
        public string ResourceName { get; set; }

        [StringLength(255)]
        public string Region { get; set; }

        public double? ConsumedQuantity { get; set; }

        public DateTime? ChargeStartDate { get; set; }

        public DateTime? ChargeEndDate { get; set; }

        [StringLength(255)]
        public string BillingProvider { get; set; }
    }
}