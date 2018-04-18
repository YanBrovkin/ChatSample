using AkkaConsoleApp.Interfaces;
using Dapper;
using System;
using System.Data;

namespace AkkaConsoleApp.DAL
{
    public class RoomRepository : IBORepository<Room>
    {
        private readonly IDbConnectionFactory db;
        public RoomRepository(IDbConnectionFactory db)
        {
            this.db = db;
        }
        public void Add(Room newObject)
        {
            using (IDbConnection conn = db.CreateConnection())
                conn.Execute(SqlHelper.InsertRoom, new { id = newObject.Id, name = newObject.Name });
        }

        public Room GetById(Guid id)
        {
            using (IDbConnection conn = db.CreateConnection())
                return conn.QueryFirstOrDefault<Room>(SqlHelper.GetRoomById, new { id = id });
        }

        public Room GetByName(string objectName)
        {
            using (IDbConnection conn = db.CreateConnection())
                return conn.QueryFirstOrDefault<Room>(SqlHelper.GetRoomByName, new { name = new DbString { Value = objectName } });
        }

        public void Update(Room updateObject)
        {
            throw new NotImplementedException();
        }
    }
}
