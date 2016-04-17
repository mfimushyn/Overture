CREATE TABLE [his].[RaceResult]
(
	[RaceResultId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY ,
	[RaceId] INT NOT NULL ,
	[CarId] INT NOT NULL ,
	[RaceEventId] BIGINT NULL ,
	[Position] TINYINT NULL,
	[DriverPoints] TINYINT NOT NULL ,
	[TeamPoints] TINYINT NOT NULL,
	CONSTRAINT [FK_RaceResult_Race] FOREIGN KEY ([RaceId]) REFERENCES [his].[Race]([RaceId]) ,
	CONSTRAINT [FK_RaceResult_Car] FOREIGN KEY ([CarId]) REFERENCES [his].[Car]([CarId]) ,
	CONSTRAINT [FK_RaceResult_RaceEvent] FOREIGN KEY ([RaceEventId]) REFERENCES [his].[RaceEvent]([RaceEventId]) ,
)
