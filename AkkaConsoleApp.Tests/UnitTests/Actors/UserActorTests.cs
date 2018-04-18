using Akka.Actor;
using AkkaConsoleApp.Messages;
using AkkaConsoleApp.Tests.Base;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;

namespace AkkaConsoleApp.Tests.UnitTests.Actors
{
    [TestFixture]
    public class UserActorTests : ActorTestBase
    {
        [Test]
        public void UserActor_should_proceed_alive_event_correctly()
        {
            //arrange
            var aliveUserEvent = new ReviveMe(@"testUser");

            //act
            var aliveUserResponse = userDispatcher.Ask<AliveUser>(aliveUserEvent).Result;

            //assert
            aliveUserResponse.ActiveUser.Should().NotBeNull();
            aliveUserResponse.ConnectedRoom.Should().NotBeNull();
            aliveUserResponse.ActiveUser.Name.Should().Be("testUser");
            aliveUserResponse.ConnectedRoom.Name.Should().Be("default");
        }

        [Test]
        public void UserActor_should_connect_to_room_properly()
        {
            //arrange
            var connectToRoom = new ConnectToRoom("testUser", "testRoom");

            //act
            var connectToRoomResponse = userDispatcher.Ask<ConnectedToRoom>(connectToRoom).Result;

            //assert
            connectToRoomResponse.Room.Should().NotBeNull();
            connectToRoomResponse.Room.Name.Should().Be("testRoom");
        }

        [Test]
        public void UserActor_should_show_received_text_message()
        {
            //arrange
            var firstUser = userDispatcher.Ask<AliveUser>(new ReviveMe("User1")).Result;
            var secondUser = userDispatcher.Ask<AliveUser>(new ReviveMe("User2")).Result;
            var messageTime = DateTime.Now;

            var message = new TextMessage
            {
                RoomId = firstUser.ConnectedRoom.Id,
                RoomName = firstUser.ConnectedRoom.Name,
                UserId = firstUser.ActiveUser.Id,
                UserName = firstUser.ActiveUser.Name,
                TimeStamp = messageTime,
                Text = "Test message"
            };

            //act
            roomDispatcher.Tell(message);

            //assert
            eventStreamSubscriber.ExpectMsg<DisplayMessages>(msgs => msgs.Messages.Any(msg => msg.Text == "Test message"));
        }
    }
}
