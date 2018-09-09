using System.Collections.Generic;

namespace Stored.Tracing
{
    public class NullTraceContext : ITraceContext
    {
        public NullTraceContext(string name) =>
            Name = name;

        public string Name { get; set; }

        public void Annotate(Dictionary<string, string> labels) { }

        public void Dispose() { }
    }
}
