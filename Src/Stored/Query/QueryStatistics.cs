using System;

namespace Stored.Query
{
    public class QueryStatistics
    {
        public QueryStatistics() => TotalCount = new Lazy<long>(() => 0);

        public Lazy<long> TotalCount { get; set; }
        public long Skip { get; set; }
        public long Take { get; set; }
    }
}
