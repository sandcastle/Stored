using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stored.Query;
using Stored.Query.Restrictions;

namespace Stored.Memory
{
    public class MemoryQuery<TResult> : QueryBase<TResult>
        where TResult : class, new()
    {
        readonly IMemorySession _session;

        public MemoryQuery(IMemorySession session) => _session = session;

        IEnumerable<TResult> GetRestricted()
        {
            if (QueryStatistics != null)
            {
                QueryStatistics.Skip = Restrictions.GetSkip();
                QueryStatistics.Take = Restrictions.GetTake();
            }

            var values = _session.Store[typeof (TResult)].Values
                .OfType<TResult>();

            foreach (var filter in Restrictions)
            {
                var filterFunction = FilterHelper.Filter<TResult>(filter);

                values = values.Where(filterFunction);
            }

            if (QueryStatistics != null)
            {
                QueryStatistics.TotalCount = new Lazy<long>(() => values.LongCount());
            }

            return values;
        }

        Task<IEnumerable<TResult>> GetRestrictedAsync() =>
            Task.FromResult(GetRestricted());

        public override IQuery<TResult> Skip(long value)
        {
            Restrictions.SetSkip(value);
            return this;
        }

        public override IQuery<TResult> Take(long value)
        {
            Restrictions.SetTake(value);
            return this;
        }

        public override Task<TResult> FirstOrDefaultAsync(CancellationToken cancel = default)
        {
            Take(1);

            var result = GetRestricted()
                .Skip((int)Restrictions.GetSkip())
                .FirstOrDefault();

            return Task.FromResult(result);
        }

        public override Task<IReadOnlyList<TResult>> ToListAsync(CancellationToken token = default)
        {
            IReadOnlyList<TResult> results = GetRestricted()
                .Skip((int)Restrictions.GetSkip())
                .Take((int)Restrictions.GetTake())
                .ToList();

            return Task.FromResult(results);
        }

        public override async Task<int> CountAsync(CancellationToken token = default)
        {
            var restricted = await GetRestrictedAsync();
            return restricted.Count();
        }

        public override async Task<long> CountLongAsync(CancellationToken token = default)
        {
            var restricted = await GetRestrictedAsync();
            return restricted.LongCount();
        }
    }
}
