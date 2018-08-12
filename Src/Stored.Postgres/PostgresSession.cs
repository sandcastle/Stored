using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Npgsql;
using Stored.Postgres.Query;
using NpgsqlTypes;

namespace Stored.Postgres
{
    public class PostgresSession : SessionBase, IPostgresSession
    {
        const int AllBatchSize = 2048;

        readonly Func<NpgsqlConnection> _connectionFactory;
        readonly PostgresSessionAdvanced _advanced;
        readonly JsonSerializerSettings _jsonSettings;

        public PostgresSession(IPostgresStore store)
        {
            Store = store;
            _connectionFactory = () =>
            {
                var connection = new NpgsqlConnection(Store.ConnectionString);
                connection.Open();

                return connection;
            };

            _jsonSettings = Store.Conventions.JsonSettings();
            _advanced = new PostgresSessionAdvanced(Store, _connectionFactory);
        }

        public IPostgresStore Store { get; }

        public override ISessionAdvanced Advanced => _advanced;

        public override IList<T> All<T>()
        {
            var table = Store.GetOrCreateTable(typeof(T));

            // NOTE: Reading records is done in batches, not a single query
            // in case there is an huge number of records

            var records = new List<T>();
            var total = 0;
            var batchSize = Store.Conventions.AllBatchSize;

            using (var connection = _connectionFactory())
            {
                while (true)
                {
                    var results = 0;

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText =
                            $@"select body from public.{table.Name} limit {batchSize} offset {total};";

                        Debug.WriteLine(command.CommandText);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                records.Add(JsonConvert.DeserializeObject<T>(reader.GetString(0), _jsonSettings));
                                total += 1;
                                results += 1;
                            }
                        }
                    }

                    if (results < AllBatchSize)
                    {
                        break;
                    }
                }
            }

            return records;
        }

        public override IQuery<T> Query<T>() => new PostgresQuery<T>(this, _connectionFactory);

        protected override T GetInternal<T>(Guid id)
        {
            var table = Store.GetOrCreateTable(typeof(T));

            using (var connection = _connectionFactory())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $@"SELECT body FROM public.{table.Name} WHERE id = :id LIMIT 1;";

                    command.Parameters.AddWithValue(":id", id);

                    Debug.WriteLine(command.CommandText);

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
            var table = Store.GetOrCreateTable(type);

            foreach (var item in items)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $@"INSERT INTO public.{table.Name} (id, body) VALUES (:id, :body);";

                    Debug.WriteLine(command.CommandText);

                    command.Parameters.AddWithValue(":id", item.Key);
                    command.Parameters.AddWithValue(":body", NpgsqlDbType.Json, JsonConvert.SerializeObject(item.Value, _jsonSettings));

                    command.ExecuteNonQuery();
                }
            }
        }

        void ExecuteModifySet(NpgsqlConnection connection, Type type, IEnumerable<KeyValuePair<Guid, object>> items)
        {
            var table = Store.GetOrCreateTable(type);

            foreach (var item in items)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        $@"UPDATE public.{table.Name} SET body = :body WHERE id = :id;";

                    Debug.WriteLine(command.CommandText);

                    command.Parameters.AddWithValue(":id", item.Key);
                    command.Parameters.AddWithValue(":body", NpgsqlDbType.Json, JsonConvert.SerializeObject(item.Value, _jsonSettings));

                    command.ExecuteNonQuery();
                }
            }
        }

        void ExecuteDeleteSet(NpgsqlConnection connection, Type type, IEnumerable<Guid> items)
        {
            var table = Store.GetOrCreateTable(type);

            foreach (var item in items)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $@"DELETE FROM public.{table.Name} WHERE id = :id;";

                    Debug.WriteLine(command.CommandText);

                    command.Parameters.AddWithValue(":id", item);

                    command.ExecuteNonQuery();
                }
            }
        }

        public class PostgresSessionAdvanced : ISessionAdvanced
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
                    using (var writer = connection.BeginBinaryImport(
                        $"COPY public.{table.Name} (id, body) FROM STDIN BINARY"))
                    {
                        foreach (var item in items)
                        {
                            var id = Guid.NewGuid();
                            try
                            {
                                ((dynamic)item).Id = id;
                            }
                            catch (RuntimeBinderException)
                            {
                                throw new Exception("Entity does not have a valid ID.");
                            }

                            var body = JsonConvert.SerializeObject(item, _store.Conventions.JsonSettings());

                            writer.StartRow();
                            writer.Write(id.ToString("N"), NpgsqlDbType.Uuid);
                            writer.Write(body, NpgsqlDbType.Json);
                        }
                    }
                }
            }
        }
    }
}
