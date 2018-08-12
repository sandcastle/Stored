using System;
using System.Diagnostics;

namespace Stored.Query.Restrictions
{
    [DebuggerDisplay("Take {" + nameof(Count) + "}")]
    public class TakeRestriction : IQueryRestriction
    {
        public TakeRestriction(long count)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), $"Take '{Count}' cant be less than 1.");
            }

            Count = count;
        }

        public long Count { get; }

        public override string ToString() => $"take {Count}";
    }
}
