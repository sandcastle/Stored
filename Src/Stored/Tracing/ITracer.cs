namespace Stored.Tracing
{
    public interface ITracer
    {
        ITraceContext Start(string name);
    }
}

// public class TraceContext : ITraceContext
// {
//     readonly Stopwatch _watch;
//
//     public TraceContext(string name)
//     {
//         Name = name;
//         _watch = new Stopwatch();
//         _watch.Start();
//     }
//
//     public string Name { get; }
//
//     public Dictionary<string, string> Labels { get; } = new Dictionary<string, string>();
//
//     public TimeSpan Duration =>
//         _watch.Elapsed;
//
//     public void Dispose() =>
//         _watch.Stop();
//
//     public void Annotate(Dictionary<string, string> labels)
//     {
//         foreach (var label in labels)
//         {
//             Labels.Add(label.Key, label.Value);
//         }
//     }
// }
