using AkkaConsoleApp.DAL;
using AkkaConsoleApp.Interfaces;

namespace AkkaConsoleApp.Messages
{
    public class ReviveMe : IHaveUserName
    {
        public string UserName { get; private set; }
        public ReviveMe(string userName)
        {
            UserName = userName;
        }
    }
    public class AliveUser
    {
        public AliveUser(User user, Room connectedRoom)
        {
            ActiveUser = user;
            ConnectedRoom = connectedRoom;
        }
        public Room ConnectedRoom { get; private set; }
        public User ActiveUser { get; private set; }
    }
}
