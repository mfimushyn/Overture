CREATE TABLE [his].[Race]
(
	[RaceId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY ,
	[TrackId] SMALLINT NOT NULL,
	[GrandPrixId] SMALLINT NOT NULL,
	CONSTRAINT [FK_Race_Track] FOREIGN KEY ([TrackId]) REFERENCES [his].[Track]([TrackId]),
	CONSTRAINT [FK_Race_GrandPrix] FOREIGN KEY ([GrandPrixId]) REFERENCES [his].[GrandPrix]([GrandPrixId])
)
