using System;
using System.Collections.Generic;
using System.Linq;

namespace Stored.Memory
{
    public class MemorySession : SessionBase, IMemorySession
    {
        readonly IMemoryStore _store;
        readonly MemorySessionAdvanced _advanced;

        public MemorySession(IMemoryStore store)
        {
            _store = store;
            _advanced = new MemorySessionAdvanced(this);
        }

        IMemoryStore IMemorySession.Store => _store;

        public override ISessionAdvanced Advanced => _advanced;

        protected override T GetInternal<T>(Guid id)
        {
            var dictionary = _store[typeof(T)];

            if (dictionary.ContainsKey(id))
            {
                return (T)dictionary[id];
            }

            return default;
        }

        public override IList<T> All<T>() =>
            _store[typeof(T)]
                .Values
                .OfType<T>()
                .ToList();

        public override IQuery<T> Query<T>() =>
            new MemoryQuery<T>(this);

        public override void Commit()
        {
            foreach (var item in Entities)
            {
                var dictionary = _store[item.Key];

                foreach (var entity in item.Value)
                {
                    dictionary[entity.Key] = entity.Value.Item1;
                }
            }

            foreach (var item in DeletedEntities)
            {
                var dictionary = _store[item.Key];

                foreach (var entity in item.Value)
                {
                    dictionary.Remove(entity.Key);
                }
            }

            Entities.Clear();
            DeletedEntities.Clear();
        }

        class MemorySessionAdvanced : ISessionAdvanced
        {
            readonly MemorySession _session;

            public MemorySessionAdvanced(MemorySession session) =>
                _session = session;

            public void BulkCreate<T>(IEnumerable<T> items)
            {
                var dictionary = _session._store[typeof (T)];

                foreach (var item in items)
                {
                    var id = IdentityFactory.SetEntityId(item);
                    dictionary[id] = item;
                }
            }
        }
    }
}
