using Akka.Actor;
using System;
using AkkaConsoleApp.Messages;
using AkkaConsoleApp.BO;
using AkkaConsoleApp.DAL;
using System.Collections.Generic;
using AkkaConsoleApp.Interfaces;

namespace AkkaConsoleApp.Actors
{
    public class UserActor : ActorBase<IHaveUserName>
    {
        private string userName;
        private UserState userState;
        private readonly IBORepository<User> userRepository;
        private readonly IBORepository<Room> roomRepository;
        private readonly IActorPathResolver pathResolver;

        public UserActor(IActorPathResolver pathResolver,
                         IBORepository<User> userRepository,
                         IBORepository<Room> roomRepository
                         )
        {
            this.pathResolver = pathResolver;
            this.userRepository = userRepository;
            this.roomRepository = roomRepository;

            Receive<ReviveMe>(me => IAmAlive(me));
            Receive<ConnectToRoom>(cmd => HandleConnectToRoom(cmd));
        }

        protected override void PreStart()
        {
            base.PreStart();

            userName = Self.Path.Name;
            userState = GetUserState(userName) ?? InitializeUserState(userName);
        }

        private UserState GetUserState(string userName)
        {
            var user = userRepository.GetByName(userName);
            if (user == null) return null;

            var stateFromDb = new UserState(userName);
            stateFromDb.SetId(user.Id);

            if (user.LastRoomId.HasValue)
            {
                var room = roomRepository.GetById(user.LastRoomId.Value);
                stateFromDb.SetLastRoom(room);
            }
            return stateFromDb;
        }

        private UserState InitializeUserState(string userName)
        {
            var initializedUserState = new UserState(userName);
            initializedUserState.SetId(Guid.NewGuid());
            userRepository.Add(new User { Id = initializedUserState.Id, Name = initializedUserState.Name });

            return initializedUserState;
        }

        private void IAmAlive(ReviveMe me)
        {
            var response = Subscribe(
                userState.LastRoom == null ? "default" : userState.LastRoom.Name,
                me.UserName);

            Sender.Tell(new AliveUser(response.Key, userState.LastRoom), Self);
        }

        private KeyValuePair<User, Room> Subscribe(string roomName, string userName)
        {
            var commandHandled = Context
                .System
                .ActorSelection(pathResolver.GetRoomDispatcherPath())
                .Ask<CommandHandled>(new Messages.Subscribe(roomName, userName))
                .Result;

            Context.System.EventStream.Publish(new ChangeCurrentRoom(roomName));

            var user = userRepository.GetByName(userName);
            var room = roomRepository.GetByName(roomName);

            userState.SetLastRoom(room);
            user.LastRoomId = room.Id;
            userRepository.Update(user);

            return new KeyValuePair<User, Room>(user, room);
        }

        private void HandleConnectToRoom(ConnectToRoom cmd)
        {
            Subscribe(cmd.RoomName, cmd.UserName);

            Sender.Tell(new ConnectedToRoom(userState.LastRoom), Self);
        }
    }
}
