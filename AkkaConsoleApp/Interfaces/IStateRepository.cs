using System;
using System.Collections.Generic;

namespace AkkaConsoleApp.Interfaces
{
    public interface IBORepository<T> : IRepository<T> where T: class
    {
        T GetById(Guid id);
        T GetByName(string objectName);
    }

    public interface IUserRoomRepository<T> : IRepository<T> where T : class
    {
        T GetByIds(Guid userId, Guid roomId);
        List<T> Get(Guid roomId);
        void Remove(T removedObject);
    }

    public interface IMessageRepository<T> : IRepository<T> where T : class
    {
        List<T> GetMessages(Guid roomId, DateTime fromTime);
    }

    public interface IRepository<T> where T : class
    {
        void Add(T newObject);
        void Update(T updateObject);
    }
}
