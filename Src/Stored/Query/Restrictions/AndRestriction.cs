using System.Collections.Generic;

namespace Stored.Query.Restrictions
{
    public class AndRestriction : ICompoundRestriction
    {
        public AndRestriction(params IUnaryRestriction[] restrictions)
        {
            foreach (var restriction in restrictions)
            {
                Restrictions.Add(restriction);
            }
        }

        public IList<IUnaryRestriction> Restrictions { get; } = new List<IUnaryRestriction>();
    }
}
