using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace Stored.Postgres
{
    public class PostgresStore : IPostgresStore
    {
        readonly string _connectionString;
        readonly IDictionary<Type, PostgresTableMetadata> _tables = new Dictionary<Type, PostgresTableMetadata>();
        static readonly object _lock = new object();

        public PostgresStore(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            _connectionString = connectionString;

            Conventions = new StoreConventions();
        }

        public StoreConventions Conventions { get; private set; }

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        PostgresTableMetadata IPostgresStore.GetOrCreateTable(Type type)
        {
            if (_tables.ContainsKey(type))
            {
                return _tables[type];
            }

            lock (_lock)
            {
                // re-check after acquiring the lock
                if (_tables.ContainsKey(type))
                {
                    return _tables[type];
                }

                var table = new PostgresTableMetadata(type);

                var connection = new NpgsqlConnection(_connectionString);
                try
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = String.Format(@"
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
                    throw new Exception(String.Format("Could not create table {0}; see the inner exception for more information.", table.Name), exception);
                }
                finally
                {
                    connection.Dispose();
                }

                _tables[type] = table;
                return table;
            }
        }

        ISession IStore.CreateSession()
        {
            return CreateSession();
        }

        public PostgresSession CreateSession()
        {
            return new PostgresSession(this);
        }
    }
}