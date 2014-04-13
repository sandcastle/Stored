using System;
using System.Collections.Generic;

namespace Stored.Memory
{
    public interface IMemoryStore : IStore
    {
        Dictionary<Guid, object> this[Type type] { get; }
    }
}
