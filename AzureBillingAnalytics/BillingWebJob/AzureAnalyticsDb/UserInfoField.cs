namespace BillingWebJob.AzureAnalyticsDb
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UserInfoField")]
    public partial class UserInfoField
    {
        public int Id { get; set; }

        public int? BillingDataId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public virtual UserBillingData UserBillingData { get; set; }
    }
}