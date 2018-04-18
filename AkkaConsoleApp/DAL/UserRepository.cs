using AkkaConsoleApp.Interfaces;
using Dapper;
using System;
using System.Data;

namespace AkkaConsoleApp.DAL
{
    public class UserRepository : IBORepository<User>
    {
        private readonly IDbConnectionFactory db;
        public UserRepository(IDbConnectionFactory db)
        {
            this.db = db;
        }
        public void Add(User newObject)
        {
            using (IDbConnection conn = db.CreateConnection())
            {
                if (newObject.LastRoomId.HasValue)
                {
                    conn.Execute(SqlHelper.InsertUserWithLastRoom, new { id = newObject.Id, name = newObject.Name, lastRoomId = newObject.LastRoomId.Value });
                    return;
                }
                conn.Execute(SqlHelper.InsertUser, new { id = newObject.Id, name = newObject.Name });
            }
        }

        public User GetById(Guid id)
        {
            using (IDbConnection conn = db.CreateConnection())
                return conn.QueryFirstOrDefault<User>(SqlHelper.GetUserById, new { id = id });
        }

        public User GetByName(string objectName)
        {
            using (IDbConnection conn = db.CreateConnection())
                return conn.QueryFirstOrDefault<User>(SqlHelper.GetUserByName, new { userName = new DbString { Value = objectName } });
        }

        public void Update(User updateObject)
        {
            using (IDbConnection conn = db.CreateConnection())
                conn.Execute(SqlHelper.UpdateUser, new { lastRoomId = updateObject.LastRoomId.Value, name = updateObject.Name });
        }
    }
}
