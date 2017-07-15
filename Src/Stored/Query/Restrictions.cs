using System.Collections.Generic;

namespace Stored.Query
{
    public class Restrictions
    {
        public Restrictions()
        {
            Filters = new List<FilterBase>();
            Skip = 0;
            Take = 1024;
            SortClause = new SortClause();
        }

        public int Skip { get; set; }
        public int Take { get; set; }
        public SortClause SortClause { get; set; }
        public List<FilterBase> Filters { get; private set; }

        public Restrictions Clone()
        {
            return new Restrictions
                {
                    Skip = Skip,
                    Take = Take,
                    SortClause = SortClause?.Clone(),
                    Filters = new List<FilterBase>(Filters) // TODO: Add deep clone
                };
        }
    }
}