using System.Collections.Generic;

namespace Stored.Query.Restrictions
{
    public interface ICompoundRestriction : IRestriction
    {
        IList<IUnaryRestriction> Restrictions { get; }
    }
}
