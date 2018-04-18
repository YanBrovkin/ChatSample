using AkkaConsoleApp.Interfaces;
using System;

namespace AkkaConsoleApp.DAL
{
    public class Message: IState
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
