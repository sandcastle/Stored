using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stored.Query
{
    public abstract class QueryBase<T> : IQuery<T> 
        where T : class, new()
    {
        readonly Restrictions _restrictions = new Restrictions();

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
            stats = new QueryStatistics();
            return this;
        }

        public IFilterBuilder<T> Where(Expression<Func<T, object>> expression)
        {
            var name = ExpressionHelper.GetName(expression);

            return new FilterBuilder(this, name);
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
                    Value = value
                });

                return _query;
            }

            public IQuery<T> NotEqual(object value)
            {
                _query.Restrictions.Filters.Add(new BinaryFilter
                {
                    FieldName = _propertyName,
                    Operator = BinaryOperator.NotEqual,
                    Value = value
                });

                return _query;
            }
        }
    }
}