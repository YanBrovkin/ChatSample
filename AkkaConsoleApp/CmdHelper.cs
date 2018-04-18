using AkkaConsoleApp.Messages;

namespace AkkaConsoleApp
{
    public static class CmdHelper
    {
        public static Cmd ParseCommand(string instruction)
        {
            if (!instruction.Contains(":")) return new Cmd { Command = instruction, Value = string.Empty };
            var cmd = instruction.Split(':');
            return new Cmd { Command = cmd[0], Value = cmd[1] };
        }
    }

    public class Cmd
    {
        public string Command { get; set; }
        public string Value { get; set; }
    }
}
