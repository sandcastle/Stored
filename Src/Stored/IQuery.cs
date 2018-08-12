using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Stored.Query;
using Stored.Query.Restrictions;

namespace Stored
{
    public interface IQuery<TResult>
    {
        IImmutableList<IRestriction> Restrictions { get; }

        IQuery<TResult> Statistics(out QueryStatistics stats);
        IQuery<TResult> Skip(long amount);
        IQuery<TResult> Take(long amount);

        Task<TResult> FirstOrDefaultAsync(CancellationToken cancel = default);
        Task<IReadOnlyList<TResult>> ToListAsync(CancellationToken cancel = default);
        Task<int> CountAsync(CancellationToken cancel = default);
        Task<long> CountLongAsync(CancellationToken cancel = default);
    }
}
