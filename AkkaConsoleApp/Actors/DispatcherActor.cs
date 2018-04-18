using Akka.Actor;
using Akka.DI.Core;
using AkkaConsoleApp.Messages;
using System;
using System.Collections.Generic;

namespace AkkaConsoleApp.Actors
{
    public class DispatcherActor<S, T> : ReceiveActor where T : ActorBase
    {
        public HashSet<string> Children;
        public DispatcherActor(Func<S, string> getChildId)
        {
            Children = new HashSet<string>();

            Receive<S>(sc =>
            {
                var childRef = GetOrCreateChildByName(getChildId(sc));
                childRef.Forward(sc);
            });
            Receive<GetChildren>(qry => 
            {
                var childrenMessage = new GetChildren();
                childrenMessage.SetChildren(Children);
                Sender.Tell(childrenMessage, Self);
            });
        }

        private IActorRef GetOrCreateChildByName(string name)
        {
            var childRef = Context.Child(name);
            if (Equals(childRef, Nobody.Instance))
                childRef = Context.ActorOf(
                    Context.System.DI().Props<T>(), name);
            Children.Add(name);
            return childRef;
        }
    }
}
