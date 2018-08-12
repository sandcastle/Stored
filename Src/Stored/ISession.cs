using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stored
{
    public interface ISession : IDisposable
    {
        ISessionAdvanced Advanced { get; }

        T Get<T>(Guid id);
        T Create<T>(T value);
        T Modify<T>(T value);
        void Delete<T>(T value);

        IQuery<T> Query<T>() where T : class, new();

        Task<IReadOnlyList<T>> AllAsync<T>(CancellationToken token = default) where T : class, new();
        Task CommitAsync(CancellationToken token = default);
    }
}
