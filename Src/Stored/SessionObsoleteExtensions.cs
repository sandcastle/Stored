using System;
using System.Collections.Generic;

namespace Stored.Query
{
    public static class SessionObsoleteExtensions
    {
        [Obsolete("Use AllAsync instead.")]
        public static IReadOnlyList<T> All<T>(this ISession session) where T : class, new()
        {
            return session.AllAsync<T>()
                .GetAwaiter()
                .GetResult();
        }

        [Obsolete("Use CommitAsync instead.")]
        public static void Commit(this ISession session)
        {
            session.CommitAsync()
                .GetAwaiter()
                .GetResult();
        }

        [Obsolete("Use BulkCreateAsync instead.")]
        public static void BulkCreate<T>(this ISessionAdvanced advanced, IEnumerable<T> items)
        {
            advanced.BulkCreateAsync(items)
                .GetAwaiter()
                .GetResult();
        }
    }
}
