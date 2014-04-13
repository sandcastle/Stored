namespace Stored.Postgres
{
    public interface IPostgresSession : ISession
    {
        IPostgresStore Store { get; }
    }
}