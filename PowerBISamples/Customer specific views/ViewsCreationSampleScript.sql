
CREATE VIEW vCustomerCurrentUsage AS
SELECT [Id]
      ,[Category]
      ,[QuantityUsed]
      ,[ResourceId]
      ,[ResourceName]
      ,[SubCategory]
      ,[TotalCost] * 1.15 as [EstimatedTotalCost]
      ,[Unit]
      ,[CustomerDomain]
      ,[SubscriptionName]
      ,[SubscriptionId]
      ,[SubscriptionStatus]
      ,[BillingStartDate]
      ,[BillingEndDate]
  FROM [dbo].[CspUsageData]
  WHERE
  [CustomerName] = 'demotenant3'

GO

CREATE VIEW vCustomerHistoricUsage AS
SELECT [Id]
      ,[UsageDate]
      ,[SubscriptionId]
      ,[SubscriptionName]
      ,[SubscriptionDescription]
      ,[ServiceName]
      ,[ServiceType]
      ,[ResourceGuid]
      ,[ResourceName]
      ,[Region]
      ,[ConsumedQuantity]
      ,[ChargeStartDate]
      ,[ChargeEndDate]
  FROM [dbo].[CspBillingData]
WHERE [CustomerCompanyName] = 'demotenant3'

GO

CREATE VIEW vCustomerBillingSummary AS
SELECT [Id]
      ,[ChargeEndDate]
      ,[ChargeStartDate]
      ,[ConsumedQuantity]
      ,[Currency]
      ,[PostTaxTotal] * 1.15 as [EstimatedCost]
      ,[Region]
      ,[ResourceGuid]
      ,[ResourceName]
      ,[ServiceName]
      ,[ServiceType]
      ,[Sku]
      ,[SubscriptionDescription]
      ,[SubscriptionId]
      ,[SubscriptionName]
  FROM [dbo].[CspSummaryData]
WHERE 
[CustomerCompanyName] = 'demotenant3'

GO