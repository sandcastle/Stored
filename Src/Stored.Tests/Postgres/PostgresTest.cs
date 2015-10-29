using System;
using System.Data;
using Npgsql;
using Stored.Postgres;
using System.Threading;

namespace Stored.Tests.Postgres
{
    public class PostgresTest : IDisposable
    {
        readonly PostgresStore _store;
        readonly PostgresSession _session;

        public PostgresTest()
        {
            Cleanup();

            _store = new PostgresStore(PostgresConfig.ConnectionString);
            _session = _store.CreateSession();
        }

        public IPostgresStore Store
        {
            get { return _store; }
        }

        public IPostgresSession Session
        {
            get { return _session; }
        }

        static void Cleanup()
        {
            using (var connection = new NpgsqlConnection(PostgresConfig.ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = String.Format("DROP SCHEMA public CASCADE;");
                    command.ExecuteNonQuery();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = String.Format("CREATE SCHEMA public;");
                    command.ExecuteNonQuery();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\" SCHEMA public;";
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Dispose()
        {
            _session.Dispose();
        }
    }
}