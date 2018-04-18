using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using AkkaConsoleApp.BO;
using AkkaConsoleApp.DAL;
using AkkaConsoleApp.Interfaces;
using AkkaConsoleApp.Messages;
using AutoMapper;
using System;
using System.Linq;

namespace AkkaConsoleApp.Actors
{
    public class RoomActor : ActorBase<IHaveRoomName>
    {
        private IActorRef mediator;
        private RoomState roomState;
        private readonly IBORepository<User> userRepository;
        private readonly IBORepository<Room> roomRepository;
        private readonly IUserRoomRepository<UserRoom> visitRepository;
        private readonly IMessageRepository<Message> messageRepository;
        private readonly IActorPathResolver pathResolver;
        public RoomActor(IBORepository<Room> roomRepository,
                         IBORepository<User> userRepository,
                         IMessageRepository<Message> messageRepository,
                         IUserRoomRepository<UserRoom> visitRepository,
                         IActorPathResolver pathResolver)
        {
            this.userRepository = userRepository;
            this.roomRepository = roomRepository;
            this.messageRepository = messageRepository;
            this.visitRepository = visitRepository;
            this.pathResolver = pathResolver;

            mediator = DistributedPubSub.Get(Context.System).Mediator;

            Receive<Messages.Subscribe>(evt => HandleSubscribe(evt));
            Receive<Messages.Unsubscribe>(evt => HandleUnsubscribe(evt));
            Receive<TextMessage>(msg => SendToSubscribers(msg));

        }

        protected override void PreStart()
        {
            base.PreStart();

            var roomName = Self.Path.Name;
            roomState = GetRoomState(roomName) ?? InitializeRoomState(roomName);
        }

        private RoomState GetRoomState(string roomName)
        {
            var room = roomRepository.GetByName(roomName);
            if (room == null) return null;

            var stateFromDb = new RoomState(roomName);
            stateFromDb.AddId(room.Id);

            var visitors = visitRepository.Get(room.Id);
            if (visitors == null || !visitors.Any())
                return stateFromDb;

            visitors.ForEach(visitor =>
            {
                var userSubscriber = userRepository.GetById(visitor.UserId.Value);
                stateFromDb.AddSubscriber(new RoomSubscription { Subscriber = userSubscriber.Name, LastVisit = visitor.LastVisitTimeStamp.Value });
            });

            return stateFromDb;
        }
        private RoomState InitializeRoomState(string roomName)
        {
            var initializedRoomState = new RoomState(roomName);
            initializedRoomState.AddId(Guid.NewGuid());
            roomRepository.Add(new Room { Id = initializedRoomState.Id, Name = initializedRoomState.RoomName });

            return initializedRoomState;
        }

        private void HandleSubscribe(Messages.Subscribe evt)
        {
            var user = userRepository.GetByName(evt.UserName);

            if (roomState.Subscribers.Any(s => s.Subscriber == evt.UserName))
            {
                var estimatedSubscriber = roomState.Subscribers.First(v => v.Subscriber == evt.UserName);
                var messages = messageRepository.GetMessages(roomState.Id, estimatedSubscriber.LastVisit);
                if (messages == null || !messages.Any())
                {
                    HandleUpdateSubscription(user.Id, roomState.Id, DateTime.Now, estimatedSubscriber);

                    Sender.Tell(new CommandHandled(true), Self);
                    return;
                }

                var maxTimeStamp = messages.Max(m => m.TimeStamp);

                HandleUpdateSubscription(user.Id, roomState.Id, maxTimeStamp, estimatedSubscriber);

                Sender.Tell(new CommandHandled(true), Self);

                messages.ForEach(msg =>
                {
                    Context.System.EventStream.Publish(new DisplayMessages(new TextMessage[]
                    {
                        new TextMessage
                        {
                            RoomId = msg.RoomId,
                            RoomName = roomState.RoomName,
                            UserId = msg.UserId,
                            UserName = msg.UserName,
                            Text = msg.Text,
                            TimeStamp = msg.TimeStamp
                        }
                    }));
                });
                return;
            }

            var unreadedMessages = messageRepository.GetMessages(roomState.Id, DateTime.Now.AddDays(-2d));
            if (unreadedMessages != null && unreadedMessages.Any())
            {
                HandleCreateSubscription(user.Id, evt.UserName, roomState.Id, unreadedMessages.Max(m => m.TimeStamp));
                Sender.Tell(new CommandHandled(true), Self);
                unreadedMessages.ForEach(msg =>
                {
                    Context.System.EventStream.Publish(new DisplayMessages(new TextMessage[]
                    {
                        new TextMessage
                        {
                            RoomId = msg.RoomId,
                            RoomName = roomState.RoomName,
                            UserId = msg.UserId,
                            UserName = msg.UserName,
                            Text = msg.Text,
                            TimeStamp = msg.TimeStamp
                        }
                    }));
                });
                return;
            }
            else
                HandleCreateSubscription(user.Id, evt.UserName, roomState.Id, DateTime.Now);

            Sender.Tell(new CommandHandled(true), Self);

            return;
        }

        private void HandleUpdateSubscription(Guid userId, Guid roomId, DateTime timeStamp, RoomSubscription subscriber)
        {
            visitRepository.Update(new UserRoom { UserId = userId, RoomId = roomId, LastVisitTimeStamp = timeStamp });
            subscriber.LastVisit = timeStamp;
        }

        private void HandleCreateSubscription(Guid userId, string userName, Guid roomId, DateTime timeStamp)
        {
            roomState.AddSubscriber(new RoomSubscription { Subscriber = userName, LastVisit = timeStamp });
            visitRepository.Add(new UserRoom { UserId = userId, RoomId = roomId, LastVisitTimeStamp = timeStamp });
        }

        private void HandleUnsubscribe(Messages.Unsubscribe evt)
        {
            if (!roomState.Subscribers.Any(s => s.Subscriber == evt.UserName))
                return;
            var user = userRepository.GetByName(evt.UserName);
            var visit = visitRepository.GetByIds(user.Id, roomState.Id);
            visitRepository.Remove(visit);
            roomState.RemoveSubscriber(evt.UserName);
            Sender.Tell(new CommandHandled(true), Self);
        }

        private void SendToSubscribers(TextMessage msg)
        {
            messageRepository.Add(
                new Message
                {
                    RoomId = msg.RoomId,
                    UserId = msg.UserId,
                    Text = msg.Text,
                    TimeStamp = msg.TimeStamp
                });

            mediator.Tell(new Publish("DisplayRoutine", new DisplayMessages(new TextMessage[]
                    {
                        new TextMessage
                        {
                            RoomId = msg.RoomId,
                            RoomName = roomState.RoomName,
                            UserId = msg.UserId,
                            UserName = msg.UserName,
                            Text = msg.Text,
                            TimeStamp = msg.TimeStamp
                        }
                    })));
        }
    }
}
