using Akka.Actor;
using AkkaConsoleApp.Actors;
using AkkaConsoleApp.BO;
using AkkaConsoleApp.DAL;
using AkkaConsoleApp.Interfaces;
using AkkaConsoleApp.Messages;
using AkkaConsoleApp.Tests.FakeMessages;
using AutoMapper;
using Ninject.Modules;
using System;

namespace AkkaConsoleApp.Tests.UnitTests
{
    public class FakeUserActor : ActorBase<IHaveUserName>
    {
        private string userName;
        public FakeUserActor()
        {
            Receive<HaveUserName>(msg =>
            {
                msg.Success();
                Sender.Tell(msg);
            });
        }
        protected override void PreStart()
        {
            base.PreStart();

            userName = Self.Path.Name;
        }
    }
    public class FakeRoomActor : ActorBase<IHaveRoomName>
    {
        private string roomName;
        public FakeRoomActor()
        {
            Receive<HaveRoomName>(msg =>
            {
                msg.Success();
                Sender.Tell(msg);
            });
        }
        protected override void PreStart()
        {
            base.PreStart();

            roomName = Self.Path.Name;
        }
    }

    public class FakeContainerModule : NinjectModule
    {
        private bool loaded;

        public override void Load()
        {
            if (loaded)
                throw new Exception("Module should be inited only once");
            loaded = true;

            //Bindings
            Bind<IActorPathResolver>().To<ActorPathResolver>();
            Bind<ActorBase<IHaveUserName>>().To<FakeUserActor>();
            Bind<ActorBase<IHaveRoomName>>().To<FakeRoomActor>();

            //Mappings
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Room, RoomState>()
                    .ForMember(d => d.RoomName, o => o.MapFrom(s => s.Name))
                    .ForMember(d => d.Subscribers, o => o.Ignore());

                cfg.CreateMap<Message, TextMessage>()
                    .ReverseMap();

            });
        }
    }
}
