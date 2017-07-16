using System;
using System.Collections.Generic;
using System.Linq;
using Stored.Query;

namespace Stored.Memory
{
    public class MemoryQuery<T> : QueryBase<T>
        where T : class, new()
    {
        readonly IMemorySession _session;

        public MemoryQuery(IMemorySession session)
        {
            _session = session;
        }

        public override T FirstOrDefault()
        {
            Take(1);

            return GetRestricted()
                .Skip(Restrictions.Skip)
                .FirstOrDefault();
        }

        public override List<T> ToList()
        {
            return GetRestricted()
                .Skip(Restrictions.Skip)
                .Take(Restrictions.Take)
                .ToList();
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
