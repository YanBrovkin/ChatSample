using AkkaConsoleApp.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AkkaConsoleApp.DAL
{
    public class MessageRepository : IMessageRepository<Message>
    {
        private readonly IDbConnectionFactory db;
        public MessageRepository(IDbConnectionFactory db)
        {
            this.db = db;
        }
        public void Add(Message newObject)
        {
            using (IDbConnection conn = db.CreateConnection())
                conn.Execute(SqlHelper.InsertMessage, new { id = Guid.NewGuid(), roomId = newObject.RoomId, userId = newObject.UserId, message = newObject.Text, timeStamp = newObject.TimeStamp });
        }

        public List<Message> GetMessages(Guid roomId, DateTime fromTime)
        {
            using (IDbConnection conn = db.CreateConnection())
                return conn.Query<Message>(SqlHelper.GetMessagesByParam, new { roomId = roomId, fromTime = fromTime }).ToList();
        }

        public void Update(Message updateObject)
        {
            throw new NotImplementedException();
        }
    }
}
