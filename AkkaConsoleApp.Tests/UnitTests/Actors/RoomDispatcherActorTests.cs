using Akka.Actor;
using AkkaConsoleApp.Messages;
using AkkaConsoleApp.Tests.Base;
using AkkaConsoleApp.Tests.FakeMessages;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;

namespace AkkaConsoleApp.Tests.UnitTests.Actors
{
    [TestFixture]
    public class RoomDispatcherActorTests: ActorDispatcherTestBase
    {
        [Test]
        public void RoomDispatcherActor_should_forward_ihaveroomname_events_to_children()
        {
            //arrange
            var ihaveusernameMsg = new HaveRoomName("testRoom");
            //act
            var result = roomDispatcher.Ask<HaveRoomName>(ihaveusernameMsg).Result;
            //assert
            result.Successful.Should().BeTrue();
        }

        [Test]
        public void RoomDispatcherActor_should_get_online_rooms()
        {
            //arrange
            var createNewOnlineRoom = new HaveRoomName("OnlineRoom");
            var getOnlineUsers = new GetChildren();
            //act
            roomDispatcher.Tell(createNewOnlineRoom);
            var onlineRoomsResponse = roomDispatcher.Ask<GetChildren>(getOnlineUsers).Result;
            //assert
            onlineRoomsResponse.Children.Count().Should().Be(1);
            onlineRoomsResponse.Children.Should().Contain("OnlineRoom");
        }
    }
}
