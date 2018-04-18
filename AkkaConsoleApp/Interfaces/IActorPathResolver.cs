namespace AkkaConsoleApp.Interfaces
{
    public interface IActorPathResolver
    {
        string RoomDispatcherName { get; }
        string UserDispatcherName { get; }

        string GetRoomDispatcherPath();
        string GetUserDispatcherPath();
    }
}
