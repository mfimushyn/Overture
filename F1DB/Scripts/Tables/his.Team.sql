CREATE TABLE [his].[Team]
(
	[TeamId] SMALLINT NOT NULL IDENTITY(1,1) PRIMARY KEY ,
	[ShortName] VARCHAR(50) NOT NULL ,
	[TeamBaseId] SMALLINT NOT NULL ,
	CONSTRAINT [FK_Team_TeamBase] FOREIGN KEY ([TeamBaseId]) REFERENCES [his].[TeamBase]([TeamBaseId])
)
