using System;
using Newtonsoft.Json;

namespace Stored.Postgres
{
    public class StoreConventions
    {
        public Func<JsonSerializerSettings> JsonSettings = () =>new JsonSerializerSettings();
    }
}
