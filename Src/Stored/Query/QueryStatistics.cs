using System;

namespace Stored.Query
{
    public class QueryStatistics
    {
        public QueryStatistics()
        {
            TotalCount = new Lazy<int>(() => 0);
        }

        public Lazy<int> TotalCount { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
