using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;
using Stored.Query;
using Stored.Tracing;

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
            using (var trace = Tracer.Trace.Start($"{nameof(ISession)}.{nameof(FirstOrDefault)}"))
            {
                trace.Annotate(new Dictionary<string, string>
                {
                    {"query/skip", Restrictions.Skip.ToString()},
                    {"query/take", "1"}
                });

                Take(1);

                return Execute(trace).FirstOrDefault();
            }
        }

        public override List<T> ToList()
        {
            using (var trace = Tracer.Trace.Start($"{nameof(ISession)}.{nameof(ToList)}"))
            {
                trace.Annotate(new Dictionary<string, string>
                {
                    { "query/skip", Restrictions.Skip.ToString() },
                    { "query/take", Restrictions.Take.ToString() }
                });

                return Execute(trace).ToList();
            }
        }

        IEnumerable<T> Execute(ITraceContext trace)
        {
            var connection = _connectionFactory();
            var command = connection.CreateCommand();

            var values = new Dictionary<string, object>();
            string query = Translate(values);

            trace.Annotate(new Dictionary<string, string>
            {
                { "query/text", query },
                { "query/type", nameof(T) },
                { "query/parameters", values.Count.ToString() }
            });

            command.CommandType = CommandType.Text;
            command.CommandText = query;

            foreach (var item in values)
            {
                command.Parameters.AddWithValue(item.Key, item.Value);
            }

            var reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            if (QueryStatistics != null)
            {
                QueryStatistics.Skip = Restrictions.Skip;
                QueryStatistics.Take = Restrictions.Take;
            }

            return new ObjectReader<T>(reader, _session.Store.Conventions.JsonSettings(), QueryStatistics);
        }

        string Translate(Dictionary<string, object> parameters) => new PostgresQueryTranslator().Translate(
            Restrictions,
            parameters,
            _metadata,
            QueryStatistics != null);

        public override string ToString() => Translate(new Dictionary<string, object>());
    }
}
