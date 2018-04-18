using AkkaConsoleApp.DAL;
using AkkaConsoleApp.Interfaces;
using System;

namespace AkkaConsoleApp.BO
{
    public class UserState : IState
    {
        public UserState(string name)
        {
            Name = name;
        }

        public void SetId(Guid id)
        {
            Id = id;
        }

        public void SetLastRoom(Room room)
        {
            LastRoom = room;
        }
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Room LastRoom { get; private set; }
    }
}
