using System;

namespace Stored.Tests.Postgres
{
    public static class PostgresConfig
    {
        public static readonly string ConnectionString;

        static PostgresConfig()
        {
            var password = GetVariable("POSTGRES_PASS", string.Empty);
            if (string.IsNullOrWhiteSpace(password) == false)
            {
                password = "Password=" + password + ";";
            }

            ConnectionString =
                $"Server={GetVariable("POSTGRES_HOST", "host")};Port={GetVariable("POSTGRES_PORT", "5432")};"
              + $"Database={GetVariable("POSTGRES_DB", "stored_db")};User Id={GetVariable("POSTGRES_USER", "postgres")};CommandTimeout=120;{password}";
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
            return string.IsNullOrWhiteSpace(value)
                ? defaultValue
                : value;
        }
    }
}