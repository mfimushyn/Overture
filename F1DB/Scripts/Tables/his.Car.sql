CREATE TABLE [his].[Car]
(
	[CarId] INT NOT NULL PRIMARY KEY ,
	[TeamId] SMALLINT NOT NULL ,
	[DriverId] INT NOT NULL ,
	CONSTRAINT [FK_Car_Team] FOREIGN KEY ([TeamId]) REFERENCES [his].[Team]([TeamId]) ,
	CONSTRAINT [FK_Car_Driver] FOREIGN KEY ([DriverId]) REFERENCES [his].[Driver]([DriverId]) ,
)
