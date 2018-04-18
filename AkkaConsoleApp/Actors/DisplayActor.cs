using System;
using Akka.Actor;
using AkkaConsoleApp.Messages;
using System.Linq;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Cluster;
using System.Threading.Tasks;

namespace AkkaConsoleApp.Actors
{
    public class DisplayActor: ReceiveActor
    {
        private IActorRef mediator;
        private string CurrentRoom;

        public DisplayActor()
        {

            mediator = DistributedPubSub.Get(Context.System).Mediator;
            mediator.Tell(new Akka.Cluster.Tools.PublishSubscribe.Subscribe("DisplayRoutine", Self));

            Receive<SubscribeAck>(_ => Become(Subscribed));
            Receive<ChangeCurrentRoom>(room => HandleChangeRoom(room));
            Receive<DisplayMessages>(msgs => Handle(msgs), msgs => CheckHandleConditions(msgs));
        }

        private void Subscribed()
        {
            Receive<ChangeCurrentRoom>(room => HandleChangeRoom(room));
            Receive<DisplayMessages>(msgs => Handle(msgs), msgs => CheckHandleConditions(msgs));
        }

        private void HandleChangeRoom(ChangeCurrentRoom room)
        {
            CurrentRoom = room.RoomName;
        }

        private void Handle(DisplayMessages msgs)
        {
            foreach (var message in msgs.Messages)
                Console.Write("\r\n[{0}] {1}: {2}", message.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss"), message.UserName, message.Text);
            Console.Write("\r\n");
        }

        private bool CheckHandleConditions(DisplayMessages msgs)
        {
            if (CurrentRoom == null || CurrentRoom == String.Empty) return false;
            if (msgs.Messages == null || !msgs.Messages.Any()) return false;

            if (!msgs.Messages.First().RoomName.SequenceEqual(CurrentRoom)) return false;
            return true;
        }
    }
}
