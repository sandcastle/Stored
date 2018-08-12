using System;

namespace Stored.Query
{
    [Obsolete("Use the new IQuery methods")]
    public interface IFilterBuilder<T>
    {
        IQuery<T> Equal(object value);
        IQuery<T> NotEqual(object value);
    }
}
