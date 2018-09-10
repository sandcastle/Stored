using System.Collections.Generic;

namespace Stored.Tracing
{
    public class NullTraceContext : ITraceContext
    {
        public void Annotate(Dictionary<string, string> labels) { }

        public void Dispose() { }
    }
}
