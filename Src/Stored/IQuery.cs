using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Stored.Query;

namespace Stored
{
    public interface IQuery<T>
    {
        IQuery<T> Take(int count);
        IQuery<T> Skip(int count);
        IQuery<T> Statistics(out QueryStatistics stats);
        IFilterBuilder<T> Where(Expression<Func<T, object>> expression);
        IFilterBuilder<T> Where(string propertyName);
        T FirstOrDefault();
        List<T> ToList();
    }
}
