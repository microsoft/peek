CREATE TABLE [dbo].[UserBillingData] (
    [ID]               INT NOT NULL IDENTITY,
    [SubscriptionId]   NVARCHAR (MAX) NULL,
    [MeterName]        NVARCHAR (MAX) NULL,
    [Region]           NVARCHAR (MAX) NULL,
    [MeterCategory]    NVARCHAR (MAX) NULL,
    [MeterSubcategory] NVARCHAR (MAX) NULL,
    [StartTime]        DATETIME       NULL,
    [EndTime]          DATETIME       NULL,
    [MeterService]     NVARCHAR (MAX) NULL,
    [MeterType]        NVARCHAR (MAX) NULL,
    [Project]          NVARCHAR (MAX) NULL,
    [Quantity]         DECIMAL (18)   NULL,
    [Total]            DECIMAL (18)   NULL, 
    CONSTRAINT [PK_billingdata] PRIMARY KEY ([ID])
);

