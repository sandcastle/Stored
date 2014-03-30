using System;

namespace Stored.Postgres
{
    public interface IPostgresStore : IStore
    {
        string ConnectionString { get; }

        PostgresTableMetadata GetOrCreateTable(Type type);
    }
}