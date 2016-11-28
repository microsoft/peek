CREATE TABLE [dbo].[AuditData]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TimeStamp] DATETIME NULL, 
    [RecordCount] INT NULL, 
    [BlobStorageUrl] NVARCHAR(MAX) NULL, 
    [Status] INT NULL, 
    [ErrorMessage] NVARCHAR(MAX) NULL, 
    [CustomerType] NVARCHAR(50) NULL
)
