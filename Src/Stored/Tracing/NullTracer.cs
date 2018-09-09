namespace Stored.Tracing
{
    public class NullTracer : ITracer
    {
        public ITraceContext Start(string name) =>
            new NullTraceContext(name);
    }
}
