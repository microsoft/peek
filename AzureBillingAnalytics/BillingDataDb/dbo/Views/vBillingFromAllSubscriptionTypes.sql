CREATE VIEW [dbo].[vBillingFromAllSubscriptionTypes]
as
(
select 
	SubscriptionId as SubscriptionId,
	'CSP' as SubscriptionType,
	Region as Region,
	ServiceName as Service,
	ServiceType as ServiceType,
	ResourceName as ResourceName,
	UsageDate as UsageDate,
	ConsumedQuantity as ConsumedQuantity,
	null as Cost
		from 
			[dbo].[CspBillingData]
UNION
	select 
	SubscriptionGuid as SubscriptionId,
	'EA' as SubscriptionType,
	ServiceSubRegion as Region,
	Service as Service,
	ServiceType as ServiceType,
	ServiceResource as ResourceName,
	Date as UsageDate,
	ResourceQtyConsumed as ConsumedQuantity,
	ExtendedCost as Cost
		from 
			[dbo].[EaBillingData]
UNION
	select 
	SubscriptionId as SubscriptionId,
	'Direct' as SubscriptionType,
	Region as Region,
	MeterCategory as Service,
	MeterSubcategory as ServiceType,
	MeterName as ResourceName,
	StartTime as UsageDate,
	Quantity as ConsumedQuantity,
	Total as Cost
		from 
			[dbo].[UserBillingData]

)
