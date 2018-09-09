namespace Stored.Tracing
{
    public static class Tracer
    {
        public static ITracer Trace { get; set; } = new NullTracer();
    }
}
