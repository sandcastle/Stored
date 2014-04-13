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
        }

        public int Skip { get; set; }
        public int Take { get; set; }
        public List<FilterBase> Filters { get; private set; }
    }
}