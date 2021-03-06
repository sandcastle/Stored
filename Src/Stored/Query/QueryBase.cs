using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Stored.Tracing;

namespace Stored.Query
{
    public abstract class QueryBase<T> : IQuery<T>, ITracedQuery
        where T : class, new()
    {
        protected Restrictions Restrictions { get; } = new Restrictions();

        protected QueryBase() =>
            ((ITracedQuery)this).Tracer = new NullTracer();

        public IQuery<T> Take(int count)
        {
            Restrictions.Take = count;
            return this;
        }

        public IQuery<T> Skip(int count)
        {
            Restrictions.Skip = count;
            return this;
        }

        public IQuery<T> Statistics(out QueryStatistics stats)
        {
            stats = QueryStatistics ?? (QueryStatistics = new QueryStatistics());
            return this;
        }

        public IFilterBuilder<T> Where(Expression<Func<T, object>> expression) =>
            new FilterBuilder(this, ExpressionHelper.GetName(expression));

        public IFilterBuilder<T> Where(string propertyName) =>
            new FilterBuilder(this, propertyName);

        public IQuery<T> OrderBy(Expression<Func<T, object>> expression, SortType sortType = SortType.Undefined, SortOrder order = SortOrder.Ascending)
        {
            string name = ExpressionHelper.GetName(expression);

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

        public IQuery<T> OrderBy(string propertyName, SortType sortType = SortType.Text, SortOrder order = SortOrder.Ascending)
        {
            Restrictions.SortClause.FieldName = propertyName.Replace("-", "").Replace("'", "");
            Restrictions.SortClause.SortOrder = order;
            Restrictions.SortClause.SortType = sortType;

            return this;
        }

        protected QueryStatistics QueryStatistics { get; private set; }

        public abstract T FirstOrDefault();

        public abstract List<T> ToList();

        class FilterBuilder : IFilterBuilder<T>
        {
            readonly QueryBase<T> _query;
            readonly string _propertyName;

            public FilterBuilder(QueryBase<T> query, string propertyName)
            {
                _query = query;
                _propertyName = propertyName;
            }

            public IQuery<T> Equal(object value)
            {
                _query.Restrictions.Filters.Add(new BinaryFilter
                {
                    FieldName = _propertyName,
                    Operator = BinaryOperator.Equal,
                    Value = TypeHelper.GetUnderlyingValue(value)
                });

                return _query;
            }

            public IQuery<T> NotEqual(object value)
            {
                _query.Restrictions.Filters.Add(new BinaryFilter
                {
                    FieldName = _propertyName,
                    Operator = BinaryOperator.NotEqual,
                    Value = TypeHelper.GetUnderlyingValue(value)
                });

                return _query;
            }
        }

        ITracer ITracedQuery.Tracer { get; set; }
    }
}
