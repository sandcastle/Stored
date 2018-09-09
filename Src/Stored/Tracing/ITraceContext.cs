using System;
using System.Collections.Generic;

namespace Stored.Tracing
{
    public interface ITraceContext : IDisposable
    {
        string Name { get; }

        void Annotate(Dictionary<string, string> labels);
    }
}
