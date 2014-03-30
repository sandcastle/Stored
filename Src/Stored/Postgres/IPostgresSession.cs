namespace Stored.Postgres
{
    public interface IPostgresSession : ISession
    {
        ISessionAdvanced Advanced { get; }
    }
}