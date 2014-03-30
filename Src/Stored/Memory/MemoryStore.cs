using System;
using System.Collections.Generic;

namespace Stored.Memory
{
    public class MemoryStore : IStore
    {
        readonly Dictionary<Type, Dictionary<Guid, object>> _entities = new Dictionary<Type, Dictionary<Guid, object>>();

        public Dictionary<Guid, object> this[Type type]
        {
            get
            {
                if (_entities.ContainsKey(type))
                {
                    return _entities[type];
                }

                var dictionary = new Dictionary<Guid, object>();
                _entities[type] = dictionary;

                return dictionary;
            }
        }

        public StoreConventions Conventions
        {
            get { throw new NotImplementedException(); }
        }

        ISession IStore.CreateSession()
        {
            return CreateSession();
        }

        public MemorySession CreateSession()
        {
            return new MemorySession(this);
        }
    }
}
