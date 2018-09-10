namespace Stored.Tracing
{
    public interface ITracer
    {
        ITraceContext Start(string name);
    }
}
