using AkkaConsoleApp.Interfaces;

namespace AkkaConsoleApp.Actors
{
    public class ActorPathResolver : IActorPathResolver
    {

        public string RoomDispatcherName { get { return "RoomDispatcher"; } }

        public string UserDispatcherName { get { return "UserDispatcher"; } }

        public string GetRoomDispatcherPath()
        {
            return string.Concat("user/", RoomDispatcherName);
        }

        public string GetUserDispatcherPath()
        {
            return string.Concat("user/", UserDispatcherName);
        }

    }
}
