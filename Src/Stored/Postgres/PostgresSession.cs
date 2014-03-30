using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Npgsql;

namespace Stored.Postgres
{
    public class PostgresSession : InMemorySession, IPostgresSession
    {
        readonly IPostgresStore _store;
        readonly Func<NpgsqlConnection> _connectionFactory;
        readonly ISessionAdvanced _advanced;
        readonly JsonSerializerSettings _jsonSettings;

        public PostgresSession(IPostgresStore store)
        {
            _store = store;
            _connectionFactory = () =>
            {
                var connection = new NpgsqlConnection(_store.ConnectionString);
                connection.Open();

                return connection;
            };

            _jsonSettings = _store.Conventions.JsonSettings();
            _advanced = new PostgresSessionAdvanced(_store, _connectionFactory);
        }

        public ISessionAdvanced Advanced
        {
            get { return _advanced; }
        }

        protected override T GetInternal<T>(Guid id)
        {
            var table = _store.GetOrCreateTable(typeof(T));

            using (var connection = _connectionFactory())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = String.Format(@"SELECT body FROM public.{0} WHERE id = :id LIMIT 1;", table.Name);

                    command.Parameters.AddWithValue(":id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return JsonConvert.DeserializeObject<T>(reader.GetString(0), _jsonSettings);
                        }

                        return default(T);
                    }
                }
            }
        }

        public override IEnumerable<T> Query<T>(IQuery query)
        {
            var table = _store.GetOrCreateTable(typeof(T));

            var values = new Dictionary<string, object>();
            var builder = new QueryBuilder(table, query, values);

            using (var connection = _connectionFactory())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = builder.Build();

                    foreach (var item in values)
                    {
                        command.Parameters.AddWithValue(item.Key, item.Value);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return JsonConvert.DeserializeObject<T>(reader.GetString(0), _jsonSettings);
                        }
                    }
                }
            }
        }

        public override void Commit()
        {
            using (var connection = _connectionFactory())
            {
                foreach (var item in Entities)
                {
                    ExecuteCreateSet(connection, item.Key, item.Value.Where(x => x.Value.Item2.IsCreate).ToDictionary(x => x.Key, y => y.Value.Item1));
                    ExecuteModifySet(connection, item.Key, item.Value.Where(x => x.Value.Item2.IsCreate == false).ToDictionary(x => x.Key, y => y.Value.Item1));
                }

                foreach (var item in DeletedEntities)
                {
                    ExecuteDeleteSet(connection, item.Key, item.Value.Select(x => x.Key));
                }
            }

            Entities.Clear();
            DeletedEntities.Clear();
        }

        void ExecuteCreateSet(NpgsqlConnection connection, Type type, IEnumerable<KeyValuePair<Guid, object>> items)
        {
            var table = _store.GetOrCreateTable(type);

            foreach (var item in items)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = String.Format(@"INSERT INTO public.{0} (id, body) VALUES (:id, :body);", table.Name);

                    command.Parameters.AddWithValue(":id", item.Key);
                    command.Parameters.AddWithValue(":body", JsonConvert.SerializeObject(item.Value, _jsonSettings));

                    command.ExecuteNonQuery();
                }
            }
        }

        void ExecuteModifySet(NpgsqlConnection connection, Type type, IEnumerable<KeyValuePair<Guid, object>> items)
        {
            var table = _store.GetOrCreateTable(type);

            foreach (var item in items)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        String.Format(@"UPDATE public.{0} SET body = :body WHERE id = :id;", table.Name);

                    command.Parameters.AddWithValue(":id", item.Key);
                    command.Parameters.AddWithValue(":body", JsonConvert.SerializeObject(item.Value, _jsonSettings));

                    command.ExecuteNonQuery();
                }
            }
        }

        void ExecuteDeleteSet(NpgsqlConnection connection, Type type, IEnumerable<Guid> items)
        {
            var table = _store.GetOrCreateTable(type);

            foreach (var item in items)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = String.Format(@"DELETE FROM public.{0} WHERE id = :id;", table.Name);

                    command.Parameters.AddWithValue(":id", item);

                    command.ExecuteNonQuery();
                }
            }
        }

        class PostgresSessionAdvanced : ISessionAdvanced
        {
            readonly IPostgresStore _store;
            readonly Func<NpgsqlConnection> _connectionFactory;

            public PostgresSessionAdvanced(
                IPostgresStore store,
                Func<NpgsqlConnection> connectionFactory)
            {
                _store = store;
                _connectionFactory = connectionFactory;
            }

            public void BulkCreate<T>(IEnumerable<T> items)
            {
                var table = _store.GetOrCreateTable(typeof(T));

                using (var connection = _connectionFactory())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = String.Format("COPY public.{0}(id, body) FROM STDIN;", table.Name);

                        var serializer = new NpgsqlCopySerializer(connection);
                        var copyIn = new NpgsqlCopyIn(command, connection, serializer.ToStream);

                        try
                        {
                            copyIn.Start();

                            foreach (var item in items)
                            {
                                var id = Guid.NewGuid();
                                try
                                {
                                    ((dynamic) item).Id = id;
                                }
                                catch (RuntimeBinderException)
                                {
                                    throw new Exception("Entity does not have a valid ID.");
                                }

                                serializer.AddString(id.ToString("N"));
                                serializer.AddString(JsonConvert.SerializeObject(item, _store.Conventions.JsonSettings()));
                                serializer.EndRow();
                                serializer.Flush();
                            }

                            copyIn.End();
                            serializer.Close();
                        }
                        catch (NpgsqlException) { }
                    }
                }
            }
        }
    }
}
