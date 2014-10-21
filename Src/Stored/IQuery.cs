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
        IQuery<T> OrderBy(Expression<Func<T, object>> expression, SortType sortType = SortType.Undefined, SortOrder order = SortOrder.Ascending);
        IQuery<T> OrderBy(string propertyName, SortType sortType = SortType.Text, SortOrder order = SortOrder.Ascending);
        T FirstOrDefault();
        List<T> ToList();
    }
}
