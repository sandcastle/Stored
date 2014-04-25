using System;
using Newtonsoft.Json;

namespace Stored.Postgres
{
    public class StoreConventions
    {
        public int AllBatchSize = 2048; 

        public Func<JsonSerializerSettings> JsonSettings = () =>new JsonSerializerSettings();
    }
}
