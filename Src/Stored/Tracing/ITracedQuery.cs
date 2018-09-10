namespace Stored.Tracing
{
    public interface ITracedQuery
    {
        ITracer Tracer { get; set; }
    }
}
