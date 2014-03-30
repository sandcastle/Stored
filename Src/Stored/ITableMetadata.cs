using System;

namespace Stored
{
    public interface ITableMetadata
    {
        Type Type { get; }
        string Name { get; }
    }
}
