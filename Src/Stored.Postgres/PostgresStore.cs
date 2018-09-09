using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace Stored.Postgres
{
    public class PostgresStore : IPostgresStore
    {
        readonly IDictionary<Type, PostgresTableMetadata> _tables = new Dictionary<Type, PostgresTableMetadata>();
        static readonly object _lock = new object();

        public PostgresStore(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            ConnectionString = connectionString;

            Conventions = new StoreConventions();
        }

        public StoreConventions Conventions { get; }

        public string ConnectionString { get; }

        PostgresTableMetadata IPostgresStore.GetOrCreateTable(Type type)
        {
            lock (_lock)
            {
                if (_tables.ContainsKey(type))
                {
                    return _tables[type];
                }

                var table = new PostgresTableMetadata(type);

                var connection = new NpgsqlConnection(ConnectionString);
                try
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = string.Format(@"
                            CREATE TABLE IF NOT EXISTS public.{0}
                            (
                                id uuid NOT NULL DEFAULT md5(random()::text || clock_timestamp()::text)::uuid,
                                body json NOT NULL,
                                created timestamp without time zone NOT NULL DEFAULT now(),
                                row_version integer NOT NULL DEFAULT 1,
                                CONSTRAINT pk_{0} PRIMARY KEY (id)
                            );", table.Name);
                        command.ExecuteNonQuery();
                    }
                }
                catch (NpgsqlException exception)
                {
                    throw new Exception(
                        $"Could not create table {table.Name}; see the inner exception for more information.",
                        exception);
                }
                finally
                {
                    connection.Dispose();
                }

                _tables[type] = table;
                return table;
            }
        }

        ISession IStore.CreateSession() =>
            CreateSession();

        public PostgresSession CreateSession() =>
            new PostgresSession(this);
    }
}
