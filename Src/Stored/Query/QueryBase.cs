using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stored.Query
{
    public abstract class QueryBase<T> : IQuery<T>
        where T : class, new()
    {
        readonly Restrictions _restrictions = new Restrictions();
        readonly QueryStatistics _stats = new QueryStatistics();

        protected Restrictions Restrictions
        {
            get { return _restrictions; }
        }

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
            stats = _stats;
            return this;
        }

        public IFilterBuilder<T> Where(Expression<Func<T, object>> expression)
        {
            return new FilterBuilder(this, ExpressionHelper.GetName(expression));
        }

        public IFilterBuilder<T> Where(string propertyName)
        {
            return new FilterBuilder(this, propertyName);
        }

        public IQuery<T> OrderBy(Expression<Func<T, object>> expression, SortType sortType = SortType.Undefined, SortOrder order = SortOrder.Ascending)
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

        public IQuery<T> OrderBy(string propertyName, SortType sortType = SortType.Text, SortOrder order = SortOrder.Ascending)
        {
            Restrictions.SortClause.FieldName = propertyName.Replace("-", "").Replace("'", "");
            Restrictions.SortClause.SortOrder = order;
            Restrictions.SortClause.SortType = sortType;

            return this;
        }

        protected QueryStatistics QueryStatistics
        {
            get { return _stats; }
        }

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
    }
}