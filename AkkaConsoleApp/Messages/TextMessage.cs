using AkkaConsoleApp.Interfaces;
using System;
using System.Collections.Generic;

namespace AkkaConsoleApp.Messages
{
    public class TextMessage: IHaveRoomName, IHaveUserName
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Text { get; set; }
    }

    public class DisplayMessages
    {
        public IEnumerable<TextMessage> Messages { get; private set; }
        public DisplayMessages(IEnumerable<TextMessage> messages)
        {
            Messages = messages;
        }
    }
}
