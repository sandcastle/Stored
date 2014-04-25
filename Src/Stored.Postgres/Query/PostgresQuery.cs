using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Npgsql;
using Stored.Query;

namespace Stored.Postgres.Query
{
    public class PostgresQuery<T> : QueryBase<T>
        where T : class, new()
    {
        readonly IPostgresSession _session;
        readonly Func<NpgsqlConnection> _connectionFactory;
        readonly PostgresTableMetadata _metadata;

        public PostgresQuery(
            IPostgresSession session, 
            Func<NpgsqlConnection> connectionFactory)
        {
            _session = session;
            _connectionFactory = connectionFactory;

            _metadata = _session.Store.GetOrCreateTable(typeof(T));
        }

        public override T FirstOrDefault()
        {
            Take(1);

            return Execute().FirstOrDefault();
        }

        public override List<T> ToList()
        {
            return Execute().ToList();
        }

        IEnumerable<T> Execute()
        {
            QueryStatistics.Skip = Restrictions.Skip;
            QueryStatistics.Take = Restrictions.Take;

            var connection = _connectionFactory();
            var command = connection.CreateCommand();
            
            var values = new Dictionary<string, object>();
            var query = Translate(values);

            command.CommandType = CommandType.Text;
            command.CommandText = query;

            foreach (var item in values)
            {
                command.Parameters.AddWithValue(item.Key, item.Value);
            }

            Debug.WriteLine(query);

            var reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            return new ObjectReader<T>(reader, _session.Store.Conventions.JsonSettings());
        }

        string Translate(Dictionary<string, object> parameters)
        {
            return new PostgresQueryTranslator().Translate(Restrictions, parameters, _metadata);
        }

        public override string ToString()
        {
            return Translate(new Dictionary<string, object>());
        }
    }
}
