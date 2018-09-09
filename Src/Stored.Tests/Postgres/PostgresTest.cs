using System;
using System.Data;
using Npgsql;
using Stored.Postgres;
using Xunit;

namespace Stored.Tests.Postgres
{
    [Collection(Tests.Postgres)]
    public class PostgresTest : IDisposable
    {
        readonly PostgresStore _store;
        readonly PostgresSession _session;

        protected PostgresTest()
        {
            Cleanup();

            _store = new PostgresStore(PostgresConfig.ConnectionString);
            _session = _store.CreateSession();
        }

        protected IPostgresStore Store =>
            _store;

        protected IPostgresSession Session =>
            _session;

        public void Dispose() =>
            _session.Dispose();

        static void Cleanup()
        {
            NpgsqlConnection connection = null;
            try
            {
                connection = new NpgsqlConnection(PostgresConfig.ConnectionString);
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"
                        drop table if exists ""car"" cascade;
                        drop table if exists ""bad"" cascade;
                        drop table if exists ""user"" cascade;
                        drop table if exists ""purchase"" cascade;
                    ";
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                connection?.Dispose();
            }
        }
    }
}
