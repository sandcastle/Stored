using System;
using System.Collections.Generic;
using System.Linq;

namespace Stored.Memory
{
    public class MemorySession : InMemorySession
    {
        readonly MemoryStore _store;
        readonly MemorySessionAdvanced _advanced;

        public MemorySession(MemoryStore store)
        {
            _store = store;
            _advanced = new MemorySessionAdvanced(this);
        }
        
        public override ISessionAdvanced Advanced
        {
            get { return _advanced; }
        }

        protected override T GetInternal<T>(Guid id)
        {
            var dictionary = _store[typeof(T)];

            if (dictionary.ContainsKey(id))
            {
                return (T)dictionary[id];
            }

            return default(T);
        }

        public override IEnumerable<T> Query<T>(IQuery query)
        {
            // TODO: Look at the performance of this..
            var values = _store[typeof(T)].Values
                .AsEnumerable();

            foreach (var item in query.Filters)
            {
                var localFilter = item;
                values = values.Where(x => MatchesFilter(x, localFilter));
            }

            return values.OfType<T>();
        }

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

        public bool MatchesFilter(object value, IQueryFilter filter)
        {
            dynamic item = value.GetType().GetProperty(filter.Name).GetValue(value, null);
            
            switch (filter.Operator)
            {
                case QueryOperator.Equal:
                    return item == filter.Value;

                case QueryOperator.NotEqual:
                    return item != filter.Value;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public class MemorySessionAdvanced : ISessionAdvanced
        {
            readonly MemorySession _session;

            public MemorySessionAdvanced(MemorySession session)
            {
                _session = session;
            }

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