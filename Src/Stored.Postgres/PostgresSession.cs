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

        readonly IPostgresStore _store;
        readonly Func<NpgsqlConnection> _connectionFactory;
        readonly PostgresSessionAdvanced _advanced;
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

        public IPostgresStore Store
        {
            get { return _store; }
        }

        public override ISessionAdvanced Advanced
        {
            get { return _advanced; }
        }

        public override IList<T> All<T>()
        {
            var table = _store.GetOrCreateTable(typeof(T));

            // NOTE: Reading records is done in batches, not a single query
            //       in case there is an huge number of records

            var records = new List<T>();
            var total = 0;
            var batchSize = _store.Conventions.AllBatchSize;

            using (var connection = _connectionFactory())
            {
                while (true)
                {
                    var results = 0;

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = String.Format(@"SELECT body FROM {0} LIMIT {1} OFFSET {2};", table.Name, batchSize, total);

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

        public override IQuery<T> Query<T>()
        {
            return new PostgresQuery<T>(this, _connectionFactory);
        }

        protected override T GetInternal<T>(Guid id)
        {
            var table = _store.GetOrCreateTable(typeof(T));

            using (var connection = _connectionFactory())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = String.Format(@"SELECT body FROM {0} WHERE id = :id LIMIT 1;", table.Name);

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
            var table = _store.GetOrCreateTable(type);

            foreach (var item in items)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = String.Format(@"INSERT INTO {0} (id, body) VALUES (:id, :body);", table.Name);

                    Debug.WriteLine(command.CommandText);

                    command.Parameters.AddWithValue(":id", item.Key);
                    command.Parameters.AddWithValue(":body", NpgsqlDbType.Json, JsonConvert.SerializeObject(item.Value, _jsonSettings));

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
                        String.Format(@"UPDATE {0} SET body = :body WHERE id = :id;", table.Name);

                    Debug.WriteLine(command.CommandText);

                    command.Parameters.AddWithValue(":id", item.Key);
                    command.Parameters.AddWithValue(":body", NpgsqlDbType.Json, JsonConvert.SerializeObject(item.Value, _jsonSettings));

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
                    command.CommandText = String.Format(@"DELETE FROM {0} WHERE id = :id;", table.Name);

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
                    using (var writer = connection.BeginBinaryImport(String.Format("COPY {0} (id, body) FROM STDIN BINARY", table.Name)))
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
