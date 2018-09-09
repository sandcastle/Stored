using System.Collections.Generic;
using System.Linq;

namespace Stored.Postgres
{
    internal static class LinqExtensions
    {
        public static List<List<T>> Partition<T>(this IEnumerable<T> list, int partitionSize)
        {
            int i = 0;
            var splits = from item in list
                group item by i++ / partitionSize
                into part
                select part.ToList();
            return splits.ToList();
        }
    }
}
