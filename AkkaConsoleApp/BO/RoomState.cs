using System;
using System.Collections.Generic;
using AkkaConsoleApp.Interfaces;

namespace AkkaConsoleApp.BO
{
    public class RoomSubscription
    {
        public string Subscriber { get; set; }
        public DateTime LastVisit { get; set; }
    }
    public class RoomState: IState
    {
        public string RoomName { get; private set; }
        public HashSet<RoomSubscription> Subscribers { get; private set; }

        public Guid Id { get; private set; }

        public RoomState(string roomName)
        {
            RoomName = roomName;
            Subscribers = new HashSet<RoomSubscription>();
        }

        public void AddId(Guid id)
        {
            Id = id;
        }

        public void AddSubscriber(RoomSubscription visitor)
        {
            Subscribers.Add(visitor);
        }

        public void RemoveSubscriber(string subscriberName)
        {
            Subscribers.RemoveWhere(s => s.Subscriber == subscriberName);
        }
    }
}
