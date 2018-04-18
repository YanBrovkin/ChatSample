using System.Collections.Generic;

namespace AkkaConsoleApp.Messages
{
    public class GetChildren
    {
        public List<string> Children { get; private set; }
        public GetChildren()
        {
            Children = new List<string>();
        }
        public void SetChildren(IEnumerable<string> children)
        {
            Children.AddRange(children);
        }
    }
}
