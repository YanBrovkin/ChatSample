using Akka.Actor;
using Akka.DI.Ninject;
using Akka.TestKit;
using Akka.TestKit.NUnit;
using AkkaConsoleApp.Actors;
using AkkaConsoleApp.Interfaces;
using AkkaConsoleApp.Tests.UnitTests;
using Ninject;
using NUnit.Framework;

namespace AkkaConsoleApp.Tests.Base
{
    public class ActorDispatcherTestBase : TestKit
    {
        internal IActorRef userDispatcher;
        internal IActorRef roomDispatcher;
        internal IKernel kernel;

        [SetUp]
        public void SetUp()
        {
            kernel = new StandardKernel(new FakeContainerModule());

            var dependencyResolver = new NinjectDependencyResolver(kernel, Sys);

            roomDispatcher = ActorOf(Props.Create(() => new DispatcherActor<IHaveRoomName, ActorBase<IHaveRoomName>>(i => i.RoomName)), @"UserDispatcher");
            userDispatcher = ActorOf(Props.Create(() => new DispatcherActor<IHaveUserName, ActorBase<IHaveUserName>>(i => i.UserName)), @"RoomDispatcher");
        }
    }
}
