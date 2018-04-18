using AkkaConsoleApp.Interfaces;

namespace AkkaConsoleApp.Tests.FakeMessages
{
    public class HaveUserName : IHaveUserName
    {
        public HaveUserName(string userName)
        {
            UserName = userName;
        }
        public void Success()
        {
            Successful = true;
        }
        public bool Successful { get; private set; }
        public string UserName { get; private set; }
    }
}
