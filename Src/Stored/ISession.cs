using System;
using System.Collections.Generic;

namespace Stored
{
    public interface ISession : IDisposable
    {
        T Get<T>(Guid id);
        T Create<T>(T value);
        T Modify<T>(T value);
        void Delete<T>(T value);
        IList<T> All<T>() where T : class, new();
        IQuery<T> Query<T>() where T : class, new();
        void Commit();
        
        ISessionAdvanced Advanced { get; }
    }
}
