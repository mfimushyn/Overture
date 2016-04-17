CREATE TABLE [his].[Driver]
(
	[DriverId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY ,
	[FirstName] VARCHAR(50) NOT NULL ,
	[LastName] VARCHAR(50) NOT NULL ,
	[FullName] AS [FirstName] + ' ' + [LastName] ,
	[CountryId] SMALLINT NOT NULL,
	[DateOfBirth] DATE NULL, 
	[PermanentNumber] TINYINT NULL,
    CONSTRAINT [FK_Driver_Country] FOREIGN KEY ([CountryId]) REFERENCES [his].[Country]([CountryId])
)
