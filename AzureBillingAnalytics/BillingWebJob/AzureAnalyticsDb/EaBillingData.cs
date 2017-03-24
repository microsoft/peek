namespace BillingWebJob.AzureAnalyticsDb
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("EaBillingData")]
    public partial class EaBillingData
    {
        public int Id { get; set; }

        [StringLength(1024)]
        public string Key { get; set; }

        [StringLength(255)]
        public string AccountOwnerId { get; set; }

        [StringLength(255)]
        public string AccountName { get; set; }

        [StringLength(255)]
        public string ServiceAdministratorId { get; set; }

        [StringLength(255)]
        public string SubscriptionId { get; set; }

        [StringLength(255)]
        public string SubscriptionGuid { get; set; }

        [StringLength(255)]
        public string SubscriptionName { get; set; }

        public DateTime? Date { get; set; }

        public int? Month { get; set; }

        public int? Day { get; set; }

        public int? Year { get; set; }

        [StringLength(255)]
        public string Product { get; set; }

        [StringLength(255)]
        public string ResourceGUID { get; set; }

        [StringLength(255)]
        public string Service { get; set; }

        [StringLength(255)]
        public string ServiceType { get; set; }

        [StringLength(255)]
        public string ServiceRegion { get; set; }

        [StringLength(1024)]
        public string DownloadUrl { get; set; }

        [StringLength(255)]
        public string ServiceResource { get; set; }

        public float? ResourceQtyConsumed { get; set; }

        public float? ResourceRate { get; set; }

        public float? ExtendedCost { get; set; }

        [StringLength(255)]
        public string ServiceSubRegion { get; set; }

        [StringLength(255)]
        public string ServiceInfo { get; set; }

        [StringLength(255)]
        public string Component { get; set; }

        [StringLength(1024)]
        public string ServiceInfo1 { get; set; }

        [StringLength(1024)]
        public string ServiceInfo2 { get; set; }

        [StringLength(1024)]
        public string AdditionalInfo { get; set; }

        [StringLength(1024)]
        public string Tags { get; set; }

        [StringLength(1024)]
        public string StoreServiceIdentifier { get; set; }

        [StringLength(255)]
        public string DepartmentName { get; set; }

        [StringLength(255)]
        public string CostCenter { get; set; }

        [StringLength(1024)]
        public string ResourceKey { get; set; }
    }
}