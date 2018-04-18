using Akka.Actor;
using Akka.Cluster;
using Akka.Configuration;
using Akka.DI.Core;
using Akka.DI.Ninject;
using AkkaConsoleApp.Actors;
using AkkaConsoleApp.DAL;
using AkkaConsoleApp.Interfaces;
using AkkaConsoleApp.Messages;
using Ninject;
using System;
using System.Linq;

namespace AkkaConsoleApp
{
    public static class StaticApplicationContext
    {

        public static ActorSystem ActorSystem { get; private set; }

        public static Room ActiveRoom { get; private set; }
        public static User ActiveUser { get; private set; }
        private static IActorRef userDispatcher;
        private static IActorRef roomDispatcher;
        private static Cluster cluster;
        public static void SendMessage(Cmd command)
        {
            if (!CheckUser()) return;
            if (!CheckRoom()) return;

            roomDispatcher.Tell(new TextMessage
            {
                RoomId = StaticApplicationContext.ActiveRoom.Id,
                RoomName = StaticApplicationContext.ActiveRoom.Name,
                UserId = StaticApplicationContext.ActiveUser.Id,
                UserName = StaticApplicationContext.ActiveUser.Name,
                Text = command.Value,
                TimeStamp = DateTime.Now
            }, roomDispatcher);
            return;
        }

        public static void ConnectToRoom(Cmd command)
        {
            if (!CheckUser()) return;
            Console.WriteLine("Welcome to room {0}", command.Value);
            var response = userDispatcher.Ask<ConnectedToRoom>(new ConnectToRoom(StaticApplicationContext.ActiveUser.Name, command.Value)).Result;
            ActiveRoom = response.Room;
        }

        internal static void GetActiveRoom()
        {
            if (CheckRoom())
                Console.WriteLine(@"ActiveRoom: {0}", ActiveRoom.Name);
        }

        public static void Introduce(ReviveMe @event)
        {
            Console.WriteLine("Hello, {0}", @event.UserName);
            var aliveResponse = userDispatcher.Ask<AliveUser>(@event).Result;

            ActiveUser = aliveResponse.ActiveUser;
            ActiveRoom = aliveResponse.ConnectedRoom;

            GetActiveRoom();
        }

        public static void DisplayRooms()
        {
            var rooms = roomDispatcher
                                    .Ask<GetChildren>(new GetChildren())
                                    .Result
                                    .Children;
            Console.WriteLine("Online rooms: {0}", rooms.Any() ? String.Join(",", rooms) : "no rooms");
        }

        public static void Startup()
        {
            ActorSystem = ActorSystem.Create("webcrawler");
            var kernel = new StandardKernel(new ContainerModule());

            var dependencyResolver = new NinjectDependencyResolver(kernel, ActorSystem);

            var actorPathResolver = kernel.Get<IActorPathResolver>();

            roomDispatcher = ActorSystem
                .ActorOf(Props.Create(() => new DispatcherActor<IHaveRoomName, ActorBase<IHaveRoomName>>(i => i.RoomName)), actorPathResolver.RoomDispatcherName);
            userDispatcher = ActorSystem
                .ActorOf(Props.Create(() => new DispatcherActor<IHaveUserName, ActorBase<IHaveUserName>>(i => i.UserName)), actorPathResolver.UserDispatcherName);

            var displayActor = ActorSystem
                .ActorOf(Props.Create(() => new DisplayActor()), "DisplayActor");

            ActorSystem.EventStream.Subscribe(displayActor, typeof(DisplayMessages));
            ActorSystem.EventStream.Subscribe(displayActor, typeof(ChangeCurrentRoom));

            cluster = Cluster.Get(ActorSystem);
        }

        public static void Shutdown()
        {
            if (cluster != null && !cluster.IsTerminated)
                cluster.Leave(cluster.SelfAddress);
            ActorSystem
                .Terminate()
                .Wait();
        }

        private static bool CheckUser()
        {
            if (ActiveUser != null) return true;
            Console.WriteLine("You should introduce yourself at first (e.g. user:John)");
            return false;
        }

        private static bool CheckRoom()
        {
            if (ActiveRoom != null) return true;
            Console.WriteLine("You should choose room (e.g. room:default)");
            return false;
        }
    }
}
