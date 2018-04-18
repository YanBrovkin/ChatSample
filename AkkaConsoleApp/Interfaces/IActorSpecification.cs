namespace AkkaConsoleApp.Interfaces
{
    public interface IActorSpecification
    { }

    public interface IHaveUserName : IActorSpecification
    {
        string UserName { get; }
    }

    public interface IHaveRoomName : IActorSpecification
    {
        string RoomName { get; }
    }
}
