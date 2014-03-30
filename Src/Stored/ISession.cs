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
        IEnumerable<T> Query<T>(IQuery query);
        void Commit();

        ISessionAdvanced Advanced { get; }
    }
}
