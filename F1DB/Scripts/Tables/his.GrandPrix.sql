CREATE TABLE [his].[GrandPrix]
(
	[GrandPrixId] SMALLINT NOT NULL IDENTITY(1,1) PRIMARY KEY ,
	[Name] VARCHAR(100) NOT NULL,
	[CountryId] SMALLINT NULL ,
	CONSTRAINT [FK_GrandPrix_Country] FOREIGN KEY ([CountryId]) REFERENCES [his].[Country]([CountryId])
)
