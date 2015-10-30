using System;
using System.Collections.Generic;

namespace Stored
{
    public interface ISessionAdvanced
    {
        void BulkCreate<T>(IEnumerable<T> items);
    }
}