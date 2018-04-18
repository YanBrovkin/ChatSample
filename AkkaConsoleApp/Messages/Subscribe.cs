using AkkaConsoleApp.DAL;
using System.Collections.Generic;
using AkkaConsoleApp.Interfaces;
using System;

namespace AkkaConsoleApp.Messages
{
    public class Subscribe: IHaveRoomName
    {
        public string RoomName { get; private set; }
        public string UserName { get; private set; }
        public Subscribe(string roomName, string userName)
        {
            RoomName = roomName;
            UserName = userName;
        }
    }

    public class Unsubscribe : IHaveRoomName
    {
        public string RoomName { get; private set; }
        public string UserName { get; private set; }
        public Unsubscribe(string roomName, string userName)
        {
            RoomName = roomName;
            UserName = userName;
        }
    }

    public class CommandHandled
    {
        public bool Success { get; private set; }
        public CommandHandled(bool successfullyHandled)
        {
            Success = successfullyHandled;
        }
    }

    public class ConnectToRoom: IHaveUserName
    {
        public string UserName { get; private set; }
        public string RoomName { get; private set; }
        public ConnectToRoom(string userName, string roomName)
        {
            UserName = userName;
            RoomName = roomName;
        }
    }

    public class ConnectedToRoom
    {
        public Room Room { get; private set; }
        public ConnectedToRoom(Room connectedRoom)
        {
            Room = connectedRoom;
        }
    }
}
