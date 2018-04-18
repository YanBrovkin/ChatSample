using Akka.Actor;
using AkkaConsoleApp.Interfaces;

namespace AkkaConsoleApp.Actors
{
    public abstract class ActorBase<T>: ReceiveActor where T: IActorSpecification
    {
    }
}
