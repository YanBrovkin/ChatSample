namespace AkkaConsoleApp.Messages
{
    public class ChangeCurrentRoom
    {
        public string RoomName { get; private set; }
        public ChangeCurrentRoom(string roomName)
        {
            RoomName = roomName;
        }
    }
}
