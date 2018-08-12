using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Stored.Query.Restrictions;

namespace Stored.Query
{
    public static class LegacyQueryExtensions
    {
        public static List<TResult> ToList<TResult>(this IQuery<TResult> query)
        {
            var result = query.ToListAsync().GetAwaiter().GetResult();
            return new List<TResult>(result);
        }

        public static TResult FirstOrDefault<TResult>(this IQuery<TResult> query) =>
            query.FirstOrDefaultAsync().GetAwaiter().GetResult();

        [Obsolete("Use new Where methods")]
        public static IFilterBuilder<TResult> Where<TResult>(this IQuery<TResult> query, Expression<Func<TResult, object>> expression)
        {
            return new FilterBuilder<TResult>(
                query,
                ExpressionHelper.GetName(expression),
                ExpressionHelper.GetPropertyType(expression));
        }

        public IQuery<T> OrderBy(Expression<Func<T, object>> expression, SortType sortType = SortType.Undefined,
            SortOrder order = SortOrder.Ascending)
        {
            var name = ExpressionHelper.GetName(expression);

            if (sortType == SortType.Undefined)
            {
                var propType = ExpressionHelper.GetPropertyType(expression);

                var theSortType = SortType.Text;
                switch (propType.ToString())
                {
                    case "System.DateTime":
                        theSortType = SortType.Date;
                        break;

                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                        theSortType = SortType.Number;
                        break;
                }

                Restrictions.SortClause.SortType = theSortType;
            }
            else
            {
                Restrictions.SortClause.SortType = sortType;
            }

            Restrictions.SortClause.FieldName = name;
            Restrictions.SortClause.SortOrder = order;

            return this;
        }

        public static IQuery<TResult> OrderBy<TResult>(
            this IQuery<TResult> query,
            string propertyName,
            SortType sortType = SortType.Text,
            SortOrder order = SortOrder.Ascending)
        {
            query.OrderByDescending()


            Restrictions.SortClause.FieldName = propertyName.Replace("-", "").Replace("'", "");
            Restrictions.SortClause.SortOrder = order;
            Restrictions.SortClause.SortType = sortType;

            return this;
        }
    }

    [Obsolete("Use the new IQuery methods")]
    internal class FilterBuilder<TResult> : IFilterBuilder<TResult>
    {
        readonly IQuery<TResult> _query;
        readonly string _propertyName;
        readonly Type _propertyType;

        public FilterBuilder(IQuery<TResult> query, string propertyName, Type propertyType)
        {
            _query = query;
            _propertyName = propertyName;
            _propertyType = propertyType;
        }

        public IQuery<TResult> Equal(object value)
        {
            _query.Restrictions.Add(new EqualRestriction(_propertyName, _propertyType, value));
            return _query;
        }

        public IQuery<TResult> NotEqual(object value)
        {
            _query.Restrictions.Add(new NotEqualRestriction(_propertyName, _propertyType, value));
            return _query;
        }
    }
}
