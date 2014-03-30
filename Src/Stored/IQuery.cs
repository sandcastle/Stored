using System.Collections.Generic;

namespace Stored
{
    public interface IQuery
    {
        int Skip { get; set; }
        int Take { get; set; }

        IList<IQueryFilter> Filters { get; }
    }
}