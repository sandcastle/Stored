using System;
using System.Collections.Generic;

namespace Stored.Tracing
{
    public interface ITraceContext : IDisposable
    {
        void Annotate(Dictionary<string, string> labels);
    }
}
