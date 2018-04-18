using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Configuration;
using Akka.DI.Ninject;
using Akka.TestKit;
using Akka.TestKit.NUnit;
using AkkaConsoleApp.Actors;
using AkkaConsoleApp.DAL;
using AkkaConsoleApp.Interfaces;
using Moq;
using Ninject;
using Ninject.Modules;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AkkaConsoleApp.Tests.Base
{
    public class TestContainerModule : NinjectModule
    {
        private bool loaded;

        public override void Load()
        {
            if (loaded)
                throw new Exception("Module should be inited only once");
            loaded = true;
        }
    }

    [TestFixture]
    public class ActorTestBase : TestKit
    {
        protected TestProbe eventStreamSubscriber;
        protected List<Room> dbRooms;
        protected List<User> dbUsers;
        protected List<UserRoom> dbVisits;
        protected List<Message> dbMessages;
        protected Mock<IBORepository<User>> userRepositoryMoq;
        protected Mock<IBORepository<Room>> roomRepositoryMoq;
        protected Mock<IUserRoomRepository<UserRoom>> visitRepositoryMoq;
        protected Mock<IMessageRepository<Message>> messageRepositoryMoq;
        protected TestActorRef<DispatcherActor<IHaveUserName, ActorBase<IHaveUserName>>> userDispatcher;
        protected TestActorRef<DispatcherActor<IHaveRoomName, ActorBase<IHaveRoomName>>> roomDispatcher;
        protected IKernel kernel;

        public ActorTestBase() : base(GetConfig()) { }

        private static Config GetConfig()
        {
            return ConfigurationFactory.ParseString("akka.actor.provider = \"Akka.Cluster.ClusterActorRefProvider, Akka.Cluster\"");
        }

        [SetUp]
        public void SetUp()
        {
            dbRooms = new List<Room>();
            dbUsers = new List<User>();
            dbVisits = new List<UserRoom>();
            dbMessages = new List<Message>();

            userRepositoryMoq = new Mock<IBORepository<User>>();
            roomRepositoryMoq = new Mock<IBORepository<Room>>();
            visitRepositoryMoq = new Mock<IUserRoomRepository<UserRoom>>();
            messageRepositoryMoq = new Mock<IMessageRepository<Message>>();

            userRepositoryMoq
                .Setup(m => m.Add(It.IsAny<User>()))
                .Callback<User>(u => dbUsers.Add(u));
            userRepositoryMoq
                .Setup(m => m.GetById(It.IsAny<Guid>()))
                .Returns<Guid>(i => dbUsers.FirstOrDefault(u => u.Id == i));
            userRepositoryMoq
                .Setup(m => m.GetByName(It.IsAny<string>()))
                .Returns<string>(n => dbUsers.FirstOrDefault(u => u.Name == n));
            userRepositoryMoq
                .Setup(m => m.Update(It.IsAny<User>()))
                .Callback<User>(u => dbUsers.First(n => n.Name == u.Name).LastRoomId = u.LastRoomId.Value);

            roomRepositoryMoq
                .Setup(m => m.Add(It.IsAny<Room>()))
                .Callback<Room>(u => dbRooms.Add(u));
            roomRepositoryMoq
                .Setup(m => m.GetById(It.IsAny<Guid>()))
                .Returns<Guid>(i => dbRooms.FirstOrDefault(u => u.Id == i));
            roomRepositoryMoq
                .Setup(m => m.GetByName(It.IsAny<string>()))
                .Returns<string>(n => dbRooms.FirstOrDefault(u => u.Name == n));

            visitRepositoryMoq
                .Setup(m => m.Add(It.IsAny<UserRoom>()))
                .Callback<UserRoom>(v => dbVisits.Add(v));
            visitRepositoryMoq
                .Setup(m => m.Get(It.IsAny<Guid>()))
                .Returns<Guid>(r => dbVisits.Where(v => v.RoomId == r).ToList());
            visitRepositoryMoq
                .Setup(m => m.GetByIds(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns<Guid, Guid>((u, r) => dbVisits.FirstOrDefault(v => v.UserId == u && v.RoomId == r));
            visitRepositoryMoq
                .Setup(m => m.Remove(It.IsAny<UserRoom>()))
                .Callback<UserRoom>(u => dbVisits.RemoveAll(v => v.UserId == u.UserId && v.RoomId == u.RoomId));

            messageRepositoryMoq
                .Setup(m => m.Add(It.IsAny<Message>()))
                .Callback<Message>(t => dbMessages.Add(t));
            messageRepositoryMoq
                .Setup(m => m.GetMessages(It.IsAny<Guid>(), It.IsAny<DateTime>()))
                .Returns<Guid, DateTime>((i, d) => dbMessages.Where(t => t.RoomId == i && t.TimeStamp > d).ToList());

            var kernelModule = new TestContainerModule();

            kernel = new StandardKernel(kernelModule);

            kernel.Bind<IBORepository<User>>().ToMethod(m => userRepositoryMoq.Object);
            kernel.Bind<IBORepository<Room>>().ToMethod(m => roomRepositoryMoq.Object);
            kernel.Bind<IUserRoomRepository<UserRoom>>().ToMethod(m => visitRepositoryMoq.Object);
            kernel.Bind<IMessageRepository<Message>>().ToMethod(m => messageRepositoryMoq.Object);
            kernel.Bind<IActorPathResolver>().To<ActorPathResolver>();
            kernel.Bind<ActorBase<IHaveUserName>>().To<UserActor>();
            kernel.Bind<ActorBase<IHaveRoomName>>().To<RoomActor>();
            var dependencyResolver = new NinjectDependencyResolver(kernel, Sys);

            roomDispatcher = ActorOfAsTestActorRef<DispatcherActor<IHaveRoomName, ActorBase<IHaveRoomName>>>(Props.Create(() => new DispatcherActor<IHaveRoomName, ActorBase<IHaveRoomName>>(i => i.RoomName)), @"RoomDispatcher");
            userDispatcher = ActorOfAsTestActorRef<DispatcherActor<IHaveUserName, ActorBase<IHaveUserName>>>(Props.Create(() => new DispatcherActor<IHaveUserName, ActorBase<IHaveUserName>>(i => i.UserName)), @"UserDispatcher");

            eventStreamSubscriber = CreateTestProbe();

            var mediator = DistributedPubSub.Get(Sys).Mediator;
            mediator.Tell(new Akka.Cluster.Tools.PublishSubscribe.Subscribe("DisplayRoutine", eventStreamSubscriber));
        }
    }
}
