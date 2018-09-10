using System;
using System.Collections.Generic;
using System.Linq;

namespace Stored.Tracing
{
    public class TracedSession : ISession
    {
        readonly ISession _session;
        readonly ITracer _tracer;

        public TracedSession(ISession session, ITracer tracer)
        {
            _session = session;
            _tracer = tracer;
        }

        public void Dispose() =>
            _session.Dispose();

        public T Create<T>(T value) =>
            _session.Create(value);

        public T Modify<T>(T value) =>
            _session.Modify(value);

        public void Delete<T>(T value) =>
            _session.Delete(value);

        public IQuery<T> Query<T>() where T : class, new()
        {
            var query = _session.Query<T>();
            ((ITracedQuery) query).Tracer = _tracer;
            return query;
        }

        public ISessionAdvanced Advanced => _session.Advanced;

        public T Get<T>(Guid id)
        {
            using (var trace = _tracer.Start($"{nameof(TracedSession)}<{typeof(T).Name}>.{nameof(Get)}"))
            {
                trace.Annotate(new Dictionary<string, string>
                {
                    { "query/id", id.ToString() }
                });

                return _session.Get<T>(id);
            }
        }

        public IList<T> All<T>() where T : class, new()
        {
            using (_tracer.Start($"{nameof(TracedSession)}<{typeof(T).Name}>.{nameof(All)}"))
            {
                return _session.All<T>();
            }
        }

        public void Commit()
        {
            using (var trace = _tracer.Start($"{nameof(TracedSession)}.{nameof(Commit)}"))
            {
                if (_session is SessionBase sessionBase)
                {
                    trace.Annotate(new Dictionary<string, string>
                    {
                        { "query/added/count", sessionBase.Entities.Sum(x =>
                            x.Value.Count(y => y.Value.Item2.IsCreate)).ToString() },
                        { "query/updated/count", sessionBase.Entities.Sum(x =>
                            x.Value.Count(y => y.Value.Item2.IsCreate == false)).ToString() },
                        { "query/deleted/count", sessionBase.DeletedEntities.Sum(x =>
                            x.Value.Count).ToString() }
                    });
                }

                _session.Commit();
            }
        }
    }
}
