CREATE TABLE [dbo].[UserInfoField]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [BillingDataId] INT NULL, 
    [Key] NVARCHAR(MAX) NULL, 
    [Value] NVARCHAR(MAX) NULL, 
    CONSTRAINT [FK_infofields_ToTable] FOREIGN KEY ([BillingDataId]) REFERENCES [UserBillingData]([ID]) 
)
