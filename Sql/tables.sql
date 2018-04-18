use ActorsSampleChat;
GO

IF OBJECT_ID('[dbo].[Messages]') IS NOT NULL
	DROP TABLE [dbo].[Messages];
IF OBJECT_ID('[dbo].[UserRooms]') IS NOT NULL
	DROP TABLE [dbo].[UserRooms];
IF OBJECT_ID('[dbo].[Rooms]') IS NOT NULL
	DROP TABLE [dbo].[Rooms];
IF OBJECT_ID('[dbo].[Users]') IS NOT NULL
	DROP TABLE [dbo].[Users];
	
CREATE TABLE [dbo].[Rooms]
(
	[id] UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
	[name] NVARCHAR(100)
);

CREATE TABLE [dbo].[Users]
(
	[id] UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
	[name] NVARCHAR(100),
	[lastRoomId] UNIQUEIDENTIFIER FOREIGN KEY REFERENCES [dbo].[Rooms]([id])
);

CREATE TABLE [dbo].[UserRooms]
(
	[userId] UNIQUEIDENTIFIER FOREIGN KEY REFERENCES [dbo].[Users]([id]),
	[roomId] UNIQUEIDENTIFIER FOREIGN KEY REFERENCES [dbo].[Rooms]([id]),
	[lastVisitTimeStamp] DATETIME
);

CREATE TABLE [dbo].[Messages]
(
	[id] UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
	[roomId] UNIQUEIDENTIFIER FOREIGN KEY REFERENCES [dbo].[Rooms]([id]),
	[userId] UNIQUEIDENTIFIER FOREIGN KEY REFERENCES [dbo].[Users]([id]),
	[message] NVARCHAR(max),
	[timeStamp] DATETIME
);