using System;

namespace BillingDataApi.Models
{
    public class CspAzureResourceUsageRecord
    {
        //
        // Summary:
        //     Gets or sets the azure resource category.
        public string Category { get; set; }
        //
        // Summary:
        //     Gets or sets the quantity of the Azure resource used.
        public decimal QuantityUsed { get; set; }
        //
        // Summary:
        //     Gets or sets the azure resource unique identifier.
        public string ResourceId { get; set; }
        //
        // Summary:
        //     Gets or sets the name of the azure resource.
        public string ResourceName { get; set; }
        //
        // Summary:
        //     Gets or sets the azure resource sub-category.
        public string Subcategory { get; set; }
        //
        // Summary:
        //     Gets or sets the estimated total cost of usage for the azure resource.
        public decimal TotalCost { get; set; }
        //
        // Summary:
        //     Gets or sets the unit of measure for the Azure resource.
        public string Unit { get; set; }

        public string CustomerName { get; set; }

        public string CustomerId { get; set; }

        public string CustomerBillingProfile { get; set; }

        public string CustomerCommerceId { get; set; }

        public string CustomerDomain { get; set; }

        public string CustomerTenantId { get; set; }

        public string CustomerRelationshipToPartner { get; set; }

        public string SubscriptionName { get; set; }

        public string SubscriptionId { get; set; }

        public string SubscriptionStatus { get; set; }

        public string SubscriptionContractType { get; set; }

        public DateTime BillingStartDate { get; set; }

        public DateTime BillingEndDate { get; set; }
    }
}