﻿using System;
using System.Collections.Generic;

namespace Stored.Memory
{
    public class MemoryStore : IMemoryStore
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

        ISession IStore.CreateSession() =>
            CreateSession();

        public MemorySession CreateSession() =>
            new MemorySession(this);
    }
}
