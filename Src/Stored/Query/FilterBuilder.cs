using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Stored.Query.Restrictions;

namespace Stored.Query
{
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
