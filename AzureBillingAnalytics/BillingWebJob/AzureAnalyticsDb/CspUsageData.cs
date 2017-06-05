namespace BillingWebJob.AzureAnalyticsDb
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CspUsageData")]
    public partial class CspUsageData
    {
        public int Id { get; set; }

        [StringLength(1024)]
        public string Category { get; set; }

        public double? QuantityUsed { get; set; }

        [StringLength(1024)]
        public string ResourceId { get; set; }

        [StringLength(1024)]
        public string ResourceName { get; set; }

        [StringLength(1024)]
        public string SubCategory { get; set; }

        public double? TotalCost { get; set; }

        [StringLength(256)]
        public string Unit { get; set; }

        [StringLength(1024)]
        public string CustomerName { get; set; }

        [StringLength(1024)]
        public string CustomerId { get; set; }

        [StringLength(1024)]
        public string CustomerCommerceId { get; set; }

        [StringLength(1024)]
        public string CustomerDomain { get; set; }

        [StringLength(1024)]
        public string CustomerTenantId { get; set; }

        [StringLength(1024)]
        public string CustomerRelationshipToPartner { get; set; }

        [StringLength(1024)]
        public string SubscriptionName { get; set; }

        [StringLength(1024)]
        public string SubscriptionId { get; set; }

        [StringLength(1024)]
        public string SubscriptionStatus { get; set; }

        [StringLength(1024)]
        public string SubscriptionContractType { get; set; }

        public DateTime? BillingStartDate { get; set; }

        public DateTime? BillingEndDate { get; set; }
    }
}