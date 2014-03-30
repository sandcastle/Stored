namespace Stored
{
    public class QueryFitler : IQueryFilter
    {
        public string Name { get; set; }
        public QueryOperator Operator { get; set; }
        public object Value { get; set; }
    }
}