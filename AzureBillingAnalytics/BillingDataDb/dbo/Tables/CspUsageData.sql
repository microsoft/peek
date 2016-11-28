CREATE TABLE [dbo].[CspUsageData]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Category] NVARCHAR(1024) NULL, 
    [QuantityUsed] FLOAT NULL, 
    [ResourceId] NVARCHAR(1024) NULL, 
    [ResourceName] NVARCHAR(1024) NULL, 
    [SubCategory] NVARCHAR(1024) NULL, 
    [TotalCost] FLOAT NULL, 
    [Unit] NVARCHAR(256) NULL, 
    [CustomerName] NVARCHAR(1024) NULL, 
    [CustomerId] NVARCHAR(1024) NULL, 
    [CustomerCommerceId] NVARCHAR(1024) NULL, 
    [CustomerDomain] NVARCHAR(1024) NULL, 
    [CustomerTenantId] NVARCHAR(1024) NULL, 
    [CustomerRelationshipToPartner] NVARCHAR(1024) NULL, 
    [SubscriptionName] NVARCHAR(1024) NULL, 
    [SubscriptionId] NVARCHAR(1024) NULL, 
    [SubscriptionStatus] NVARCHAR(1024) NULL, 
    [SubscriptionContractType] NVARCHAR(1024) NULL, 
    [BillingStartDate] DATETIME NULL, 
    [BillingEndDate] DATETIME NULL,

)
