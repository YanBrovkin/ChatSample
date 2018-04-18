using Dapper;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using AkkaConsoleApp.Interfaces;

namespace AkkaConsoleApp.DAL
{
    public class UserRoomRepository : IUserRoomRepository<UserRoom>
    {
        private readonly IDbConnectionFactory db;
        public UserRoomRepository(IDbConnectionFactory db)
        {
            this.db = db;
        }

        public void Add(UserRoom newObject)
        {
            using (IDbConnection conn = db.CreateConnection())
                conn.Execute(SqlHelper.InsertVisit, new { userId = newObject.UserId.Value, roomId = newObject.RoomId.Value, lastVisitTimeStamp = newObject.LastVisitTimeStamp.Value });
        }

        public UserRoom GetByIds(Guid userId, Guid roomId)
        {
            using (IDbConnection conn = db.CreateConnection())
                return conn.QueryFirstOrDefault<UserRoom>(SqlHelper.GetLastVisitByRoomIdAndUserId, new { userId = userId, roomId = roomId });
        }

        public void Update(UserRoom updateObject)
        {
            using (IDbConnection conn = db.CreateConnection())
                conn.Execute(SqlHelper.UpdateLastVisitByUserAndRoomIds, new { lastVisitTimeStamp = updateObject.LastVisitTimeStamp.Value, userId = updateObject.UserId, roomId = updateObject.RoomId });
        }

        public List<UserRoom> Get(Guid roomId)
        {
            using (IDbConnection conn = db.CreateConnection())
                return conn.Query<UserRoom>(SqlHelper.GetSubscribersByRoomId, new { roomId = roomId }).ToList();
        }

        public void Remove(UserRoom removedObject)
        {
            using (IDbConnection conn = db.CreateConnection())
                conn.Execute(SqlHelper.DeleteLastVisitByUserAndRoomIds, new { userId = removedObject.UserId, roomId = removedObject.RoomId });
        }
    }
}
