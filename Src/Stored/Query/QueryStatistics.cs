using System;

namespace Stored.Query
{
    public class QueryStatistics
    {
        public QueryStatistics()
        {
            TotalCount = new Lazy<long>(() => 0);
        }

        public Lazy<long> TotalCount { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
