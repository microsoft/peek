namespace BillingWebJob.AzureAnalyticsDb
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CspSummaryData")]
    public partial class CspSummaryData
    {
        public int Id { get; set; }

        public DateTime? ChargeEndDate { get; set; }

        public DateTime? ChargeStartDate { get; set; }

        [StringLength(1024)]
        public string ChargeType { get; set; }

        public double? ConsumedQuantity { get; set; }

        public double? ConsumptionDiscount { get; set; }

        public double? ConsumptionPrice { get; set; }

        [StringLength(50)]
        public string Currency { get; set; }

        [StringLength(1024)]
        public string CustomerCompanyName { get; set; }

        public int? DetailLineItemId { get; set; }

        public double? IncludedQuantity { get; set; }

        [StringLength(50)]
        public string InvoiceNumber { get; set; }

        public double? ListPrice { get; set; }

        public int? MpnId { get; set; }

        [StringLength(50)]
        public string OrderId { get; set; }

        public double? OverageQuantity { get; set; }

        [StringLength(50)]
        public string PartnerBillingAccountId { get; set; }

        [StringLength(50)]
        public string PartnerId { get; set; }

        [StringLength(256)]
        public string PartnerName { get; set; }

        public double? PostTaxEffectiveRate { get; set; }

        public double? PostTaxTotal { get; set; }

        public double? PreTaxCharges { get; set; }

        public double? PreTaxEffectiveRate { get; set; }

        [StringLength(50)]
        public string Region { get; set; }

        [StringLength(50)]
        public string ResourceGuid { get; set; }

        [StringLength(256)]
        public string ResourceName { get; set; }

        [StringLength(256)]
        public string ServiceName { get; set; }

        [StringLength(256)]
        public string ServiceType { get; set; }

        [StringLength(50)]
        public string Sku { get; set; }

        [StringLength(1024)]
        public string SubscriptionDescription { get; set; }

        [StringLength(50)]
        public string SubscriptionId { get; set; }

        [StringLength(256)]
        public string SubscriptionName { get; set; }

        public double? TaxAmount { get; set; }

        public int? Tier2MpnId { get; set; }
    }
}