using System.Collections.Generic;

namespace Stored
{
    public class Query : IQuery
    {
        readonly IList<IQueryFilter> _filters = new List<IQueryFilter>();

        public int Skip { get; set; }
        public int Take { get; set; }

        public IList<IQueryFilter> Filters
        {
            get { return _filters; }
        }
    }
}