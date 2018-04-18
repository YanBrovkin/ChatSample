using Ninject.Modules;
using System;
using System.Configuration;
using AkkaConsoleApp.Actors;
using AkkaConsoleApp.DAL;
using AkkaConsoleApp.BO;
using AutoMapper;
using AkkaConsoleApp.Messages;
using AkkaConsoleApp.Interfaces;

namespace AkkaConsoleApp
{
    public class ContainerModule : NinjectModule
    {
        private bool loaded;

        public override void Load()
        {
            if (loaded)
                throw new Exception("Module should be inited only once");
            loaded = true;

            //Bindings
            var connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
            Bind<IDbConnectionFactory>()
                    .To<SqlConnectionFactory>()
                    .WithConstructorArgument("connectionString", connectionString);

            Bind<IBORepository<User>>().To<UserRepository>();
            Bind<IBORepository<Room>>().To<RoomRepository>();
            Bind<IUserRoomRepository<UserRoom>>().To<UserRoomRepository>();
            Bind<IMessageRepository<Message>>().To<MessageRepository>();
            Bind<IActorPathResolver>().To<ActorPathResolver>();
            Bind<ActorBase<IHaveUserName>>().To<UserActor>();
            Bind<ActorBase<IHaveRoomName>>().To<RoomActor>();

            //Mappings
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Room, RoomState>()
                    .ForMember(d => d.RoomName, o => o.MapFrom(s => s.Name))
                    .ForMember(d => d.Subscribers, o => o.Ignore());

                cfg.CreateMap<Message, TextMessage>()
                    .ReverseMap();

            });
        }
    }
}
