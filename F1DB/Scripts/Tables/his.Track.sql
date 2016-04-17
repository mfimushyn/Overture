CREATE TABLE [his].[Track]
(
	[TrackId] SMALLINT NOT NULL IDENTITY(1,1) PRIMARY KEY ,
	[Name] VARCHAR(100) NOT NULL,
	[CountryId] SMALLINT NOT NULL,
	[City] VARCHAR(100) NULL,
	[Location] GEOGRAPHY NULL, 
    CONSTRAINT [FK_Track_Country] FOREIGN KEY ([CountryId]) REFERENCES [his].[Country]([CountryId])
)
