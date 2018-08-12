using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stored
{
    public static class QuerySyncExtensions
    {
        [Obsolete("Use the ToListAsync method.")]
        public static IReadOnlyList<TResult> ToList<TModel, TResult>(this IQuery<TModel> query)
        {
            return query.ToListAsync<TResult>()
                .GetAwaiter()
                .GetResult();
        }

        [Obsolete("Use the OrderBy/OrderByDescending methods.")]
        public static IQuery<TModel> OrderBy<TModel>(
            this IQuery<TModel> query,
            Expression<Func<TModel, object>> expression,
            SortType sortType = SortType.Undefined,
            SortOrder order = SortOrder.Ascending)
        {
            // TODO: Add new OrderBy/OrderByDescending

            return query;
        }

        [Obsolete("Use the OrderBy/OrderByDescending methods.")]
        public static IQuery<TModel> OrderBy<TModel>(
            this IQuery<TModel> query,
            string propertyName,
            SortType sortType = SortType.Text,
            SortOrder order = SortOrder.Ascending)
        {
            // TODO: Add new OrderBy/OrderByDescending

            return query;
        }
    }
}
