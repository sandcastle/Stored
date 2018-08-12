using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stored
{
    public static class QueryExtensions
    {
        public static async Task<QueryPage<TResult>> ToPageAsync<TResult>(
            IQuery<TResult> query,
            long take,
            long skip = 0,
            CancellationToken token = default)
        {
            var results = await query
                .Statistics(out var stats)
                .Take(take)
                .Skip(skip)
                .ToListAsync<TResult>(token);

            return new QueryPage<TResult>(results, stats.TotalCount.Value, take, skip);
        }

        public static async Task<TResult> FirstAsync<TResult>(
            IQuery<TResult> query,
            CancellationToken token = default)
        {
            var results = await query
                .Take(1)
                .ToListAsync<TResult>(token);

            if (results.Count != 1)
            {
                throw new InvalidOperationException("The source sequence is empty.");
            }

            return results[0];
        }

        public static async Task<TResult> FirstOrDefaultAsync<TResult>(
            IQuery<TResult> query,
            CancellationToken token = default)
        {
            var results = await query
                .Take(1)
                .ToListAsync<TResult>(token);

            return results.Count == 1
                ? results[0]
                : default;
        }

        public static async Task<TResult> SingleAsync<TResult>(
            IQuery<TResult> query,
            CancellationToken token = default)
        {
            var results = await query
                .ToListAsync<TResult>(token);

            if (results.Count == 0)
            {
                throw new InvalidOperationException("No element satisfies the condition in predicate.");
            }

            if (results.Count > 1)
            {
                throw new InvalidOperationException("More than one element satisfies the condition in predicate.");
            }

            return results[0];
        }

        public static async Task<TResult> SingleOrDefaultAsync<TResult>(
            IQuery<TResult> query,
            CancellationToken token = default)
        {
            var results = await query
                .ToListAsync<TResult>(token);

            return results.Count == 1
                ? results[0]
                : default;
        }
    }
}
