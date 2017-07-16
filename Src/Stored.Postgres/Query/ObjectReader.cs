using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Npgsql;
using Stored.Query;

namespace Stored.Postgres.Query
{
    internal class ObjectReader<T> : IEnumerable<T>
        where T : class, new()
    {
        Enumerator _enumerator;

        internal ObjectReader(
            NpgsqlDataReader reader,
            JsonSerializerSettings settings,
            QueryStatistics statistics)
        {
            _enumerator = new Enumerator(reader, settings, statistics);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var enumerator = _enumerator;

            if (enumerator == null)
            {
                throw new InvalidOperationException("Cannot enumerate more than once");
            }

            _enumerator = null;
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        class Enumerator : IEnumerator<T>
        {
            long _row;
            readonly NpgsqlDataReader _reader;
            readonly JsonSerializerSettings _jsonSettings;
            readonly QueryStatistics _statistics;

            internal Enumerator(
                NpgsqlDataReader reader,
                JsonSerializerSettings jsonSettings,
                QueryStatistics statistics)
            {
                _reader = reader;
                _jsonSettings = jsonSettings;
                _statistics = statistics;
            }

            public T Current { get; private set; }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                _row++;

                if (!_reader.Read())
                {
                    return false;
                }

                Current = JsonConvert.DeserializeObject(_reader.GetString(0), typeof(T), _jsonSettings) as T;

                if (_row != 1 || _statistics == null)
                {
                    return true;
                }

                var totalRows = _reader.GetInt64(1);
                _statistics.TotalCount = new Lazy<long>(() => totalRows);

                return true;
            }

            public void Reset() { }

            public void Dispose()
            {
                _reader.Dispose();
            }
        }
    }
}