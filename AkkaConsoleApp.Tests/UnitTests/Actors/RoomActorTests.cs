using Akka.Actor;
using AkkaConsoleApp.DAL;
using AkkaConsoleApp.Messages;
using AkkaConsoleApp.Tests.Base;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AkkaConsoleApp.Tests.UnitTests.Actors
{
    [TestFixture]
    public class RoomActorTests : ActorTestBase
    {
        [Test]
        public void RoomActor_should_properly_subscribe_user()
        {
            //arrange
            dbUsers.Add(new User { Id = Guid.Parse("{29EEA961-CF24-40C9-A80E-CF4C22D3D71B}"), Name = "User1" });

            var subscriptionResult = roomDispatcher.Ask<CommandHandled>(new Subscribe("default", "User1")).Result;

            var createdRoom = dbRooms.FirstOrDefault(r => r.Name == "default");
            subscriptionResult
                .Success
                .Should()
                .BeTrue();
            dbVisits
                .Any(v => v.UserId.Value == Guid.Parse("{29EEA961-CF24-40C9-A80E-CF4C22D3D71B}") && v.RoomId.Value == createdRoom.Id)
                .Should()
                .BeTrue();
        }

        [Test]
        public void RoomActor_should_properly_unsubscribe_user()
        {
            dbUsers.Add(new User { Id = Guid.Parse("{29EEA961-CF24-40C9-A80E-CF4C22D3D71B}"), Name = "User1" });

            var subscriptionResult = roomDispatcher.Ask<CommandHandled>(new Subscribe("default", "User1")).Result;

            var createdRoom = dbRooms.FirstOrDefault(r => r.Name == "default");

            dbVisits
                .Any(v => v.UserId.Value == Guid.Parse("{29EEA961-CF24-40C9-A80E-CF4C22D3D71B}") && v.RoomId.Value == createdRoom.Id)
                .Should()
                .BeTrue();

            var unsubscribeResult = roomDispatcher.Ask<CommandHandled>(new Unsubscribe("default", "User1")).Result;
            unsubscribeResult
                .Success
                .Should()
                .BeTrue();
        }

        [Test]
        public void RoomActor_should_properly_send_message_to_all_subscribers()
        {
            //arrange
            var aliveUser1 = userDispatcher.Ask<AliveUser>(new ReviveMe("User1")).Result;
            var aliveUser2 = userDispatcher.Ask<AliveUser>(new ReviveMe("User2")).Result;

            Task.WaitAll(
                roomDispatcher.Ask<CommandHandled>(new Subscribe("default", "User1")),
                roomDispatcher.Ask<CommandHandled>(new Subscribe("default", "User2"))
            );

            var room = dbRooms.FirstOrDefault(r => r.Name == "default");
            var currentTimeStamp = DateTime.Now;

            //act
            roomDispatcher.Tell(
                new TextMessage
                {
                    RoomId = room.Id,
                    RoomName = "default",
                    UserId = Guid.Parse("{29EEA961-CF24-40C9-A80E-CF4C22D3D71B}"),
                    UserName = "User1",
                    Text = "Send to subscribers",
                    TimeStamp = currentTimeStamp
                });

            //assert
            eventStreamSubscriber.ExpectMsg<DisplayMessages>(msgs => msgs.Messages.Any(msg => msg.Text == "Send to subscribers"));
        }
    }
}
