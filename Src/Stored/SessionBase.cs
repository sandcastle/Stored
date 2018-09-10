using System;
using System.Collections.Generic;
using Stored.Tracing;

namespace Stored
{
    public abstract class SessionBase : ISession
    {
        static readonly object _getLock = new object();
        static readonly object _deleteLock = new object();

        protected internal readonly Dictionary<Type, Dictionary<Guid, object>> DeletedEntities
            = new Dictionary<Type, Dictionary<Guid, object>>();
        protected internal readonly Dictionary<Type, Dictionary<Guid, Tuple<object, EntityMetadata>>> Entities
            = new Dictionary<Type, Dictionary<Guid, Tuple<object, EntityMetadata>>>();

        protected SessionBase() =>
            Id = Guid.NewGuid();

        public Guid Id { get; }

        public abstract ISessionAdvanced Advanced { get; }

        public abstract void Commit();

        protected abstract T GetInternal<T>(Guid id);

        public abstract IList<T> All<T>()
            where T : class, new();

        public abstract IQuery<T> Query<T>()
            where T : class, new();

        public void Clear()
        {
            lock (_getLock)
            {
                Entities.Clear();
            }

            lock (_deleteLock)
            {
                DeletedEntities.Clear();
            }
        }

        public T Get<T>(Guid id)
        {
            var entities = GetLocalEntities<T>();
            lock (_getLock)
            {
                if (entities.ContainsKey(id))
                {
                    return (T) entities[id].Item1;
                }
            }

            return GetInternal<T>(id);
        }

        public T Create<T>(T value)
        {
            var entities = GetLocalEntities<T>();

            var id = IdentityFactory.SetEntityId(value);
            lock (_getLock)
            {
                entities[id] = new Tuple<object, EntityMetadata>(value, new EntityMetadata{ IsCreate = true});
            }

            RemoveFromDeleteIfExists<T>(id);

            return value;
        }

        public T Modify<T>(T value)
        {
            var entities = GetLocalEntities<T>();

            var id = IdentityFactory.GetEntityId(value);
            lock (_getLock)
            {
                entities[id] = new Tuple<object, EntityMetadata>(value, new EntityMetadata {IsCreate = false});
            }

            return value;
        }

        public void Delete<T>(T value)
        {
            var id = IdentityFactory.GetEntityId(value);

            // if we are attempting to delete an item that has been
            // added or updated in the unit of work, it should just be removed
            var entities = GetLocalEntities<T>();
            if (entities.ContainsKey(id))
            {
                entities.Remove(id);
            }

            var deleted = GetLocalDeletedEntities<T>();

            lock (_deleteLock)
            {
                if (deleted.ContainsKey(id))
                {
                    return;
                }

                deleted.Add(id, value);
            }
        }

        public virtual void Dispose() { }

        Dictionary<Guid, object> GetLocalDeletedEntities<T>()
        {
            lock (_deleteLock)
            {
                var type = typeof(T);
                if (DeletedEntities.ContainsKey(type))
                {
                    return DeletedEntities[type];
                }

                var dictionary = new Dictionary<Guid, object>();
                DeletedEntities[type] = dictionary;

                return dictionary;
            }
        }

        Dictionary<Guid, Tuple<object, EntityMetadata>> GetLocalEntities<T>()
        {
            lock (_getLock)
            {
                var type = typeof(T);
                if (Entities.ContainsKey(type))
                {
                    return Entities[type];
                }

                var dictionary = new Dictionary<Guid, Tuple<object, EntityMetadata>>();
                Entities[type] = dictionary;

                return dictionary;
            }
        }

        void RemoveFromDeleteIfExists<T>(Guid id)
        {
            var deleted = GetLocalDeletedEntities<T>();

            lock (_deleteLock)
            {
                if (deleted.ContainsKey(id))
                {
                    deleted.Remove(id);
                }
            }
        }
    }
}
