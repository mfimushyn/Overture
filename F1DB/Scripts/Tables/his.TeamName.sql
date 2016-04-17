CREATE TABLE [his].[TeamName]
(
	[TeamId] SMALLINT NOT NULL IDENTITY(1,1)  ,
	[Year] SMALLINT NOT NULL  ,
	[Name] VARCHAR(100) NOT NULL ,
    CONSTRAINT [PK_TeamName] PRIMARY KEY ([TeamId], [Year]) ,
	CONSTRAINT [FK_TeamName_Team] FOREIGN KEY ([TeamId]) REFERENCES [his].[Team]([TeamId]), 
)
