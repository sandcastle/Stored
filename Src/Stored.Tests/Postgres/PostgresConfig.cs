using System;
using System.IO;

namespace Stored.Tests.Postgres
{
    public static class PostgresConfig
    {
        public readonly static string ConnectionString;

        static PostgresConfig()
        {
            var password = GetVariable("POSTGRES_PASS", String.Empty);
            if (String.IsNullOrWhiteSpace(password) == false)
            {
                password = "Password=" + password + ";";
            }

            ConnectionString = String.Format("Server={0};Port={1};Database={2};User Id={3};{4}",
                GetVariable("POSTGRES_HOST", "192.168.99.100"),
                GetVariable("POSTGRES_PORT", "5432"),
                GetVariable("POSTGRES_DB", "stored_db"),
                GetVariable("POSTGRES_USER", "postgres"),
                password);
        }
        
        /// <summary>
        /// Gets the specified variable, or the default value if not availables.
        /// </summary>
        /// <returns>The variable.</returns>
        /// <param name="name">Name.</param>
        /// <param name="defaultValue">Default value.</param>
        static string GetVariable(string name, string defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (String.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return value;
        }
    }
}