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
            return GetRestricted()
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
            var values = _session.Store[typeof (T)]
                .Values
                .OfType<T>();

            foreach (var filter in Restrictions.Filters)
            {
                var filterFunction = FilterHelper.Filter<T>(filter);

                foreach (var item in values)
                {
                    if (filterFunction(item) == false)
                    {
                        continue;
                    }

                    yield return item;
                }
            }
        }
    }
}
