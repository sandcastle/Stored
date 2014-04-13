using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Npgsql;

namespace Stored.Postgres.Query
{
    internal class ObjectReader<T> : IEnumerable<T> 
        where T : class, new()
    {
        Enumerator _enumerator;

        internal ObjectReader(
            NpgsqlDataReader reader,
            JsonSerializerSettings settings)
        {
            _enumerator = new Enumerator(reader, settings);
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
            readonly NpgsqlDataReader _reader;
            readonly JsonSerializerSettings _jsonSettings;

            internal Enumerator(
                NpgsqlDataReader reader,
                JsonSerializerSettings jsonSettings)
            {
                _reader = reader;
                _jsonSettings = jsonSettings;
            }

            public T Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (_reader.Read())
                {
                    Current = JsonConvert.DeserializeObject(_reader.GetString(0), typeof(T), _jsonSettings) as T;
                    return true;
                }

                return false;
            }

            public void Reset() { }

            public void Dispose()
            {
                _reader.Dispose();
            }
        }
    }
}