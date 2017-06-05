namespace BillingWebJob.AzureAnalyticsDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UserBillingData")]
    public partial class UserBillingData
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage",
             "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserBillingData()
        {
            UserInfoFields = new HashSet<UserInfoField>();
        }

        public int ID { get; set; }

        public string SubscriptionId { get; set; }

        public string MeterName { get; set; }

        public string Region { get; set; }

        public string MeterCategory { get; set; }

        public string MeterSubcategory { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string MeterService { get; set; }

        public string MeterType { get; set; }

        public string Project { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? Total { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage",
             "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserInfoField> UserInfoFields { get; set; }
    }
}