using System;
using Newtonsoft.Json;

namespace Stored.Postgres
{
    public class StoreConventions
    {
        static readonly JsonSerializerSettings _defaultSettings = new JsonSerializerSettings();

        public int AllBatchSize = 2048;

        public Func<JsonSerializerSettings> JsonSettings = () => _defaultSettings;
    }
}
