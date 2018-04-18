using AkkaConsoleApp.Messages;
using System;

namespace AkkaConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            String instruction;
            Boolean quitNow = false;

            StaticApplicationContext.Startup();

            while (!quitNow)
            {
                Console.WriteLine("Commands: ");
                Console.WriteLine("         get rooms       - show online rooms");
                Console.WriteLine("         user:<username> - login as <username>");
                Console.WriteLine("         room:<roomname> - connect to room <roomname>");
                Console.WriteLine("         message:<text>  - send text message to active room");
                Console.WriteLine("         active room     - show active room");
                Console.WriteLine("         quit            - quit");
                Console.WriteLine();
                Console.Write("Command>");
                instruction = Console.ReadLine();
                var command = CmdHelper.ParseCommand(instruction);
                switch (command.Command)
                {
                    case "get rooms":
                        StaticApplicationContext.DisplayRooms();
                        break;
                    case "user":
                        StaticApplicationContext.Introduce(new ReviveMe(command.Value));
                        break;
                    case "room":
                        StaticApplicationContext.ConnectToRoom(command);
                        break;
                    case "message":
                        StaticApplicationContext.SendMessage(command);
                        break;
                    case "quit":
                        quitNow = true;
                        break;
                    case "active room":
                        StaticApplicationContext.GetActiveRoom();
                        break;
                    default:
                        Console.WriteLine(@"Unknown command!");
                        break;
                }
            }

            StaticApplicationContext.Shutdown();
        }
    }

}
