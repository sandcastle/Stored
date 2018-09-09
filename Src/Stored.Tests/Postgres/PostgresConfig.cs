using System;
using Npgsql;

namespace Stored.Tests.Postgres
{
    public static class PostgresConfig
    {
        public static readonly string ConnectionString;

        static PostgresConfig()
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Timeout = 120,
                Host = GetVariable("POSTGRES_HOST", "localhost"),
                Database = GetVariable("POSTGRES_DB", "stored_db"),
                Username = GetVariable("POSTGRES_USER", "postgres"),
                Port = int.Parse(GetVariable("POSTGRES_PORT", "5432"))
            };

            string password = GetVariable("POSTGRES_PASS", string.Empty);
            if (string.IsNullOrWhiteSpace(password) == false)
            {
                builder.Password = password;
            }

            ConnectionString = builder.ToString();
        }

        /// <summary>
        /// Gets the specified variable, or the default value if not available.
        /// </summary>
        /// <returns>The variable.</returns>
        /// <param name="name">Name.</param>
        /// <param name="defaultValue">Default value.</param>
        static string GetVariable(string name, string defaultValue)
        {
            string value = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrWhiteSpace(value)
                ? defaultValue
                : value;
        }
    }
}
