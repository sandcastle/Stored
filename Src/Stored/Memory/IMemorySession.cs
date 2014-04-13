namespace Stored.Memory
{
    public interface IMemorySession : ISession
    {
        IMemoryStore Store { get; }
    }
}