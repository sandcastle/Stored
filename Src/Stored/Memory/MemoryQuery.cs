using System;
using System.Collections.Generic;
using System.Linq;
using Stored.Query;
using Stored.Tracing;

namespace Stored.Memory
{
    public class MemoryQuery<T> : QueryBase<T>
        where T : class, new()
    {
        readonly IMemorySession _session;

        public MemoryQuery(IMemorySession session) =>
            _session = session;

        public override T FirstOrDefault()
        {
            using (var trace = Tracer.Trace.Start($"{nameof(ISession)}.{nameof(FirstOrDefault)}"))
            {
                trace.Annotate(new Dictionary<string, string>
                {
                    { "query/skip", Restrictions.Skip.ToString() },
                    { "query/take", "1" }
                });

                Take(1);

                return GetRestricted()
                    .Skip(Restrictions.Skip)
                    .FirstOrDefault();
            }
        }

        public override List<T> ToList()
        {
            using (var trace = Tracer.Trace.Start($"{nameof(ISession)}.{nameof(ToList)}"))
            {
                trace.Annotate(new Dictionary<string, string>
                {
                    { "query/skip", Restrictions.Skip.ToString() },
                    { "query/take", Restrictions.Take.ToString() }
                });

                return GetRestricted()
                    .Skip(Restrictions.Skip)
                    .Take(Restrictions.Take)
                    .ToList();
            }
        }

        IEnumerable<T> GetRestricted()
        {
            if (QueryStatistics != null)
            {
                QueryStatistics.Skip = Restrictions.Skip;
                QueryStatistics.Take = Restrictions.Take;
            }

            var values = _session.Store[typeof (T)].Values
                .OfType<T>();

            foreach (var filter in Restrictions.Filters)
            {
                var filterFunction = FilterHelper.Filter<T>(filter);

                values = values.Where(filterFunction);
            }

            if (QueryStatistics != null)
            {
                QueryStatistics.Skip = Restrictions.Skip;
                QueryStatistics.Take = Restrictions.Take;
                QueryStatistics.TotalCount = new Lazy<long>(() => values.LongCount());
            }

            return values;
        }
    }
}
