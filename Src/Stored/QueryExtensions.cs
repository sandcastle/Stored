using System.Collections.Generic;

namespace Stored
{
    public static class QueryExtensions
    {
        public static IEnumerable<IQueryFilter> WithEqual(this IList<IQueryFilter> filters, string name, object value)
        {
            filters.Add(new QueryFitler
            {
                Name = name, 
                Operator = QueryOperator.Equal, 
                Value = value
            });

            return filters;
        }
    }
}