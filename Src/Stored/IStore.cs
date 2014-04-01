namespace Stored
{
    public interface IStore
    {
        ISession CreateSession();
    }
}