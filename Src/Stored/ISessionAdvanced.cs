using System;
using System.Collections.Generic;

namespace Stored
{
    public interface ISessionAdvanced
    {
        [Obsolete("This is currentlyl broken in v3 of NpgSql")]
        void BulkCreate<T>(IEnumerable<T> items);
    }
}