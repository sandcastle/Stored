using System;
using System.Diagnostics;

namespace Stored.Query.Restrictions
{
    [DebuggerDisplay("Skip {" + nameof(Count) + "}")]
    public class SkipRestriction : IQueryRestriction
    {
        public SkipRestriction(long count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), $"Skip '{Count}' cant be less than 0.");
            }

            Count = count;
        }

        public long Count { get; }

        public override string ToString() => $"take {Count}";
    }
}
