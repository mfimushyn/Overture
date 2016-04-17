CREATE TABLE [his].[Lap]
(
	[RaceId] INT NOT NULL  ,
	[LapId] SMALLINT NOT NULL  ,
	[CarId] INT NOT NULL  ,
	[TimeTicks] BIGINT NULL ,
    CONSTRAINT [PK_Lap] PRIMARY KEY ([RaceId],[LapId],[CarId]),
	CONSTRAINT [FK_Lap_Car] FOREIGN KEY ([CarId]) REFERENCES [his].[Car]([CarId]) ,
	CONSTRAINT [FK_Lap_Race] FOREIGN KEY ([RaceId]) REFERENCES [his].[Race]([RaceId]), 
)
