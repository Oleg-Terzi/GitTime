USE Local_GitTime
GO

IF EXISTS(SELECT * FROM sys.tables WHERE name = 'AccessToken' AND SCHEMA_NAME([schema_id]) = 'c')
    DROP TABLE c.AccessToken;
GO

CREATE TABLE c.AccessToken (
    pk_ID int NOT NULL CONSTRAINT PK_AccessToken PRIMARY KEY IDENTITY (1,1)
   ,fk_ContactID int NOT NULL CONSTRAINT FK_AccessToken__ContactID FOREIGN KEY REFERENCES c.Contact(pk_ID)
   ,[Application] nvarchar(16) NOT NULL
   ,[Key] nvarchar(max) NOT NULL
   ,UtcCreated datetime NOT NULL
)
GO

CREATE UNIQUE INDEX UNQ_AccessToken ON c.AccessToken(fk_ContactID,[Application]);
GO