using AkkaConsoleApp.BO;
using AkkaConsoleApp.Interfaces;
using System;

namespace AkkaConsoleApp.DAL
{
    public class User: IState
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? LastRoomId { get; set; }
    }

    public class Room: IState
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public override bool Equals(Object r)
        {
            if (this == null || r == null)
                return false;

            if (System.Object.ReferenceEquals(this, r))
                return true;

            Room p = r as Room;
            if ((Object)p == null)
                return false;
            return (Id == p.Id) && (Name == p.Name);
        }
    }

}
