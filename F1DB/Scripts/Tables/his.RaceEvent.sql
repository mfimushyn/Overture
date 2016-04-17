CREATE TABLE [his].[RaceEvent]
(
	[RaceEventId] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY ,
	[RaceEventTypeId] TINYINT NULL ,
	[Description] VARCHAR(200) NULL ,
	CONSTRAINT [FK_RaceEvent_TeamBase] FOREIGN KEY ([RaceEventId]) REFERENCES [his].[RaceEvent]([RaceEventId])
)
