using System;
using Newtonsoft.Json;

namespace Stored
{
    public class StoreConventions
    {
        public Func<JsonSerializerSettings> JsonSettings = () =>new JsonSerializerSettings();
    }
}
