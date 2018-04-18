using AkkaConsoleApp.Interfaces;

namespace AkkaConsoleApp.Tests.FakeMessages
{
    public class HaveRoomName : IHaveRoomName
    {
        public HaveRoomName(string name)
        {
            RoomName = name;
        }
        public void Success()
        {
            Successful = true;
        }
        public bool Successful { get; private set; }
        public string RoomName { get; private set; }
    }
}
