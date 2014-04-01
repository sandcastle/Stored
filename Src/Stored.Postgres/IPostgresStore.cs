using System;

namespace Stored.Postgres
{
    public interface IPostgresStore : IStore
    {
        string ConnectionString { get; }

        StoreConventions Conventions { get; }

        PostgresTableMetadata GetOrCreateTable(Type type);
    }
}