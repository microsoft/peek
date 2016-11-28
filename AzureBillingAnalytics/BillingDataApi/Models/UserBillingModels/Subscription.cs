using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingDataApi.Models
{
    public class Subscription
    {
        public string Id { get; set; }

        [NotMapped]
        public string DisplayName { get; set; }

        [NotMapped]
        public string OrganizationId { get; set; }

        [NotMapped]
        public bool IsConnected { get; set; }

        public string UsageDetails { get; set; }

        [NotMapped]
        public RootObject usagePayload { get; set; }

        [NotMapped]
        public DateTime ConnectedOn { get; set; }

        public string ConnectedBy { get; set; }

        [NotMapped]
        public bool AzureAccessNeedsToBeRepaired { get; set; }

        public IEnumerable<ResourceTotal> ResourceTotals { get; set; }
        public IEnumerable<ProjectUsage> ProjectTotals { get; set; }
        public IEnumerable MeterIdTotals { get; set; }
        public IEnumerable StorageTotalsByDate { get; set; }
        public IEnumerable<ServiceTotal> ServiceTotals { get; set; }
        public IEnumerable<RatedUsage> ratedUsage { get; set; }

        public IEnumerable<ProjectRate> ProjectRates { get; set; }

        public IEnumerable<ProjectEstimate> ProjectEstimateTotals { get; set; }

        public IEnumerable<ResourceEstimate> ResourceEstimates { get; set; }
        public IEnumerable<string> Projects { get; set; }
        public double RatedEstimate { get; set; }
    }
}