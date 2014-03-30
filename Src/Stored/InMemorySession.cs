using System;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;

namespace Stored
{
    public abstract class InMemorySession : ISession
    {
        protected readonly Dictionary<Type, Dictionary<Guid, object>> DeletedEntities = new Dictionary<Type, Dictionary<Guid, object>>();
        protected readonly Dictionary<Type, Dictionary<Guid, Tuple<object, EntityMetadata>>> Entities = new Dictionary<Type, Dictionary<Guid, Tuple<object, EntityMetadata>>>();

        protected InMemorySession()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// The session ID.
        /// </summary>
        public Guid Id { get; private set; }

        public abstract void Commit();

        protected abstract T GetInternal<T>(Guid id);

        public abstract IEnumerable<T> Query<T>(IQuery query);

        public void Clear()
        {
            Entities.Clear();
            DeletedEntities.Clear();
        }

        public T Get<T>(Guid id)
        {
            var entities = GetLocalEntities<T>();
            if (entities.ContainsKey(id))
            {
                return (T)entities[id].Item1;
            }

            return GetInternal<T>(id);
        }

        public T Create<T>(T value)
        {
            var entities = GetLocalEntities<T>();

            var id = SetEntityId(value);
            entities[id] = new Tuple<object, EntityMetadata>(value, new EntityMetadata{ IsCreate = true});

            RemoveFromDeleteIfExists<T>(id);

            return value;
        }

        public T Modify<T>(T value)
        {
            var entities = GetLocalEntities<T>();

            var id = GetEntityId(value);
            entities[id] = new Tuple<object, EntityMetadata>(value, new EntityMetadata { IsCreate = false });

            return value;
        }

        public void Delete<T>(T value)
        {
            var entities = GetLocalEntities<T>();

            var id = GetEntityId(value);
            if (entities.ContainsKey(id) == false)
            {
                throw new Exception("Cannot delete an entity that is not associated with the session.");
            }

            var deleted = GetLocalDeletedEntities<T>();
            if (deleted.ContainsKey(id))
            {
                return;
            }

            deleted.Add(id, value);
        }

        public virtual void Dispose() { }

        Dictionary<Guid, object> GetLocalDeletedEntities<T>()
        {
            var type = typeof (T);
            if (DeletedEntities.ContainsKey(type))
            {
                return DeletedEntities[type];
            }

            var dictionary = new Dictionary<Guid, object>();
            DeletedEntities[type] = dictionary;

            return dictionary;
        }

        Dictionary<Guid, Tuple<object, EntityMetadata>> GetLocalEntities<T>()
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

        void RemoveFromDeleteIfExists<T>(Guid id)
        {
            var deleted = GetLocalDeletedEntities<T>();
            if (deleted.ContainsKey(id))
            {
                deleted.Remove(id);
            }
        }

        static Guid SetEntityId(dynamic value)
        {
            try
            {
                var id = Guid.NewGuid();
                value.Id = id;

                return id;
            }
            catch (RuntimeBinderException)
            {
                throw new Exception("Entity does not have an ID property.");
            }
        }

        static Guid GetEntityId(dynamic value)
        {
            try
            {
                Guid id = value.Id;
                if (id == Guid.Empty)
                {
                    throw new Exception("Entity ID cannot be Empty.");
                }

                return id;
            }
            catch (RuntimeBinderException)
            {
                throw new Exception("Entity does not have an ID property.");
            }
        }
    }
}