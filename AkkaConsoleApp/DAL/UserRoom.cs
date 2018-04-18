using System;

namespace AkkaConsoleApp.DAL
{
    public class UserRoom
    {
        public Guid? UserId { get; set; }
        public Guid? RoomId { get; set; }
        public DateTime? LastVisitTimeStamp { get; set; }
    }
}
