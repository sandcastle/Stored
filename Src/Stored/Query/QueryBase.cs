using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Stored.Query.Restrictions;

namespace Stored.Query
{
    public abstract class QueryBase<TResult> : IQuery<TResult>
        where TResult : class, new()
    {
        public IImmutableList<IRestriction> Restrictions { get; } = ImmutableList.Create<IRestriction>();

        protected QueryStatistics QueryStatistics { get; private set; }

        public abstract IQuery<TResult> Skip(long amount);

        public abstract IQuery<TResult> Take(long amount);

        public abstract Task<TResult> FirstOrDefaultAsync(CancellationToken cancel = default);

        public abstract Task<IReadOnlyList<TResult>> ToListAsync(CancellationToken cancel = default);

        public abstract Task<int> CountAsync(CancellationToken cancel = default);

        public abstract Task<long> CountLongAsync(CancellationToken cancel = default);

        public IQuery<TResult> Statistics(out QueryStatistics stats)
        {
            stats = QueryStatistics ?? (QueryStatistics = new QueryStatistics());
            return this;
        }
    }
}
