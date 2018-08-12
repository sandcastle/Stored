using System.Collections.Generic;

namespace Stored
{
    public class QueryPage<TResult>
    {
        public QueryPage(IEnumerable<TResult> items, long totalItems, long skip, long take)
        {
            Items = new List<TResult>(items);
            TotalItems = totalItems;
            Skip = skip;
            Take = take;
        }

        public IReadOnlyList<TResult> Items { get; }
        public long TotalItems { get; }
        public long Skip { get; }
        public long Take { get; }

        public long PageNumber => Skip <= 0 || Take <= 0
            ? 1
            : Skip / Take + 1;

        public long PageCount => TotalItems / Take;
        public bool IsFirstPage => PageNumber == 1;
        public bool IsLastPage => PageNumber == PageCount;
        public bool HasNextPage => PageNumber < PageCount;
        public bool HasPreviousPage => PageNumber > 1;
    }
}