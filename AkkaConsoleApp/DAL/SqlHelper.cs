namespace AkkaConsoleApp.DAL
{
    public static class SqlHelper
    {
        public const string GetUserById = @"
                select 
                    usr.id as Id,
                    usr.name as Name,
                    usr.lastRoomId as LastRoomId
                from dbo.Users as usr
                where usr.id = @id"; 

        public const string GetUserByName = @"
                select 
                    usr.id as Id,
                    usr.name as Name,
                    usr.lastRoomId as LastRoomId
                from dbo.Users as usr
                where usr.name = @userName";

        public const string GetRoomById = @"
                select
                    rm.id as Id,
                    rm.name as Name
                from dbo.Rooms as rm
                where rm.id = @id"; 

        public const string GetRoomByName = @"
                select
                    rm.id as Id,
                    rm.name as Name
                from dbo.Rooms as rm
                where rm.name = @name"; 

        public const string GetLastVisitByRoomIdAndUserId = @"
                select
                    v.userId as UserId,
                    v.roomId as RoomId,
                    v.lastVisitTimeStamp as LastVisitTimeStamp
                from dbo.UserRooms as v
                where v.userId = @userId and v.roomId = @roomId"; 

        public const string GetSubscribersByRoomId = @"
                select
                    visit.userId as UserId,
                    visit.roomId as RoomId,
                    visit.lastVisitTimeStamp as LastVisitTimeStamp
                from dbo.UserRooms as visit
                where visit.roomId = @roomId"; 

        public const string GetMessagesByParam = @"
                select 
                    mess.id as Id,
                    mess.roomId as RoomId,
                    rm.name as RoomName,
                    mess.userId as UserId,
                    usr.name as UserName,
                    mess.message as Text,
                    mess.timeStamp as TimeStamp
                from dbo.Messages as mess
                join dbo.Users as usr on usr.id = mess.userId
                join dbo.Rooms as rm on rm.id = mess.roomId
                where mess.roomId = @roomId and mess.timeStamp > @fromTime"; 

        public const string InsertUser = @"insert into dbo.Users(id, name) values(@id, @name)"; 

        public const string InsertUserWithLastRoom = @"insert into dbo.Users(id, name, lastRoomId) values(@id, @name, @lastRoomId)";

        public const string InsertRoom = @"insert into dbo.Rooms(id, name) values(@id, @name)";

        public const string InsertVisit = @"insert into dbo.UserRooms(userId, roomId, lastVisitTimeStamp) values (@userId, @roomId, @lastVisitTimeStamp)"; 

        public const string InsertMessage = @"insert into dbo.Messages(id, roomId, userId, message, timeStamp) values (@id, @roomId, @userId, @message, @timeStamp)";

        public const string UpdateUser = @"update dbo.Users set lastRoomId = @lastRoomId where name = @name"; 

        public const string UpdateLastVisitByUserAndRoomIds = @"
                update dbo.UserRooms
                set lastVisitTimeStamp = @lastVisitTimeStamp
                where userId = @userId and roomId = @roomId"; 

        public const string DeleteLastVisitByUserAndRoomIds = @"
                delete from dbo.UserRooms
                where userId = @userId and roomId = @roomId";
    }
}
