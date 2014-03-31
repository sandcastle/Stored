﻿using System;
using System.Data;
using Npgsql;
using Stored.Postgres;

namespace Stored.Tests.Postgres
{
    public class PostgresTest : IDisposable
    {
        const string ConnectionString = "Server=127.0.0.1;Port=5433;Database=integration_test;User Id=test_user;Password=123456;";

        readonly PostgresStore _store;
        readonly PostgresSession _session;

        public PostgresTest()
        {
            Cleanup();

            _store = new PostgresStore(ConnectionString);
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

                    command.CommandText = String.Format("CREATE SCHEMA public;");
                    command.ExecuteNonQuery();
                    
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