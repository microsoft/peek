namespace BillingWebJob.AzureAnalyticsDb
{
    using System.Data.Entity;

    public partial class AzureAnalyticsDbModel : DbContext
    {
        public AzureAnalyticsDbModel()
            : base("name=AzureAnalyticsDbModel")
        {
        }

        public virtual DbSet<AuditData> AuditDatas { get; set; }
        public virtual DbSet<CspBillingData> CspBillingDatas { get; set; }
        public virtual DbSet<CspSummaryData> CspSummaryDatas { get; set; }
        public virtual DbSet<CspUsageData> CspUsageDatas { get; set; }
        public virtual DbSet<EaBillingData> EaBillingDatas { get; set; }
        public virtual DbSet<UserBillingData> UserBillingDatas { get; set; }
        public virtual DbSet<UserInfoField> UserInfoFields { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EaBillingData>()
                .Property(e => e.Key)
                .IsUnicode(false);

            modelBuilder.Entity<EaBillingData>()
                .Property(e => e.CostCenter)
                .IsFixedLength();

            modelBuilder.Entity<UserBillingData>()
                .Property(e => e.Quantity)
                .HasPrecision(18, 0);

            modelBuilder.Entity<UserBillingData>()
                .Property(e => e.Total)
                .HasPrecision(18, 0);

            modelBuilder.Entity<UserBillingData>()
                .HasMany(e => e.UserInfoFields)
                .WithOptional(e => e.UserBillingData)
                .HasForeignKey(e => e.BillingDataId);
        }
    }
}