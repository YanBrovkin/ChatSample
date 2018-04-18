using Akka.Actor;
using NUnit.Framework;
using FluentAssertions;
using System.Linq;
using AkkaConsoleApp.Tests.FakeMessages;
using AkkaConsoleApp.Messages;
using AkkaConsoleApp.Tests.Base;

namespace AkkaConsoleApp.Tests.UnitTests.Actors
{
    [TestFixture]
    public class UserDispatcherActorTests : ActorDispatcherTestBase
    {
        [Test]
        public void UserDispatcherActor_should_forward_ihaveusername_events_to_children()
        {
            //arrange
            var ihaveusernameMsg = new HaveUserName("testUser");
            //act
            var result = userDispatcher.Ask<HaveUserName>(ihaveusernameMsg).Result;
            //assert
            result.Successful.Should().BeTrue();
        }

        [Test]
        public void UserDispatcherActor_should_get_online_users()
        {
            //arrange
            var createNewOnlineUser = new HaveUserName("OnlineUser1");
            var getOnlineUsers = new GetChildren();
            //act
            userDispatcher.Tell(createNewOnlineUser);
            var onlineUsersResponse = userDispatcher.Ask<GetChildren>(getOnlineUsers).Result;
            //assert
            onlineUsersResponse.Children.Count().Should().Be(1);
            onlineUsersResponse.Children.Should().Contain("OnlineUser1");
        }
    }
}
