using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stored
{
    public static class QuerySyncExtensions
    {
        public static List<TResult> ToList<TResult>(this IQuery<TResult> query)
        {
            var result = query.ToListAsync().GetAwaiter().GetResult();
            return new List<TResult>(result);
        }

        public static TResult FirstOrDefault<TResult>(this IQuery<TResult> query) =>
            query.FirstOrDefaultAsync().GetAwaiter().GetResult();

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

        // public IQuery<T> OrderBy(Expression<Func<T, object>> expression, SortType sortType = SortType.Undefined,
        //     SortOrder order = SortOrder.Ascending)
        // {
        //     var name = ExpressionHelper.GetName(expression);
        //
        //     if (sortType == SortType.Undefined)
        //     {
        //         var propType = ExpressionHelper.GetPropertyType(expression);
        //
        //         var theSortType = SortType.Text;
        //         switch (propType.ToString())
        //         {
        //             case "System.DateTime":
        //                 theSortType = SortType.Date;
        //                 break;
        //
        //             case "System.Int16":
        //             case "System.Int32":
        //             case "System.Int64":
        //                 theSortType = SortType.Number;
        //                 break;
        //         }
        //
        //         Restrictions.SortClause.SortType = theSortType;
        //     }
        //     else
        //     {
        //         Restrictions.SortClause.SortType = sortType;
        //     }
        //
        //     Restrictions.SortClause.FieldName = name;
        //     Restrictions.SortClause.SortOrder = order;
        //
        //     return this;
        // }
        //
        // public static IQuery<TResult> OrderBy<TResult>(
        //     this IQuery<TResult> query,
        //     string propertyName,
        //     SortType sortType = SortType.Text,
        //     SortOrder order = SortOrder.Ascending)
        // {
        //     Restrictions.SortClause.FieldName = propertyName.Replace("-", "").Replace("'", "");
        //     Restrictions.SortClause.SortOrder = order;
        //     Restrictions.SortClause.SortType = sortType;
        //
        //     return this;
        // }
    }
}
