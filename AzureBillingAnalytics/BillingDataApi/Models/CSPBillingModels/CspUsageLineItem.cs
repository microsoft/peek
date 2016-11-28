using System;

namespace BillingDataApi.Models
{
    public class CspUsageLineItem
    {
        public int Id { get; set; }

        public DateTime? UsageDate { get; set; }

        public string CustomerBillableAccount { get; set; }

        public string PartnerId { get; set; }

        public string PartnerName { get; set; }

        public string PartnerBillableAccountId { get; set; }

        public string CustomerCompanyName { get; set; }

        public string MpnId { get; set; }

        public string TierMpnId { get; set; }

        public string invoiceNumber { get; set; }

        public string SubscriptionId { get; set; }

        public string SubscriptionName { get; set; }

        public string SubscriptionDescription { get; set; }

        public string orderId { get; set; }

        public string ServiceName { get; set; }

        public string ServiceType { get; set; }

        public string ResourceGuid { get; set; }

        public string ResourceName { get; set; }

        public string Region { get; set; }

        public double? ConsumedQuantity { get; set; }

        public DateTime? ChargeStartDate { get; set; }

        public DateTime? ChargeEndDate { get; set; }

        public string BillingProvider { get; set; }
    }
}