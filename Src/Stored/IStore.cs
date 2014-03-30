namespace Stored
{
    public interface IStore
    {
        StoreConventions Conventions { get; }

        ISession CreateSession();
    }
}