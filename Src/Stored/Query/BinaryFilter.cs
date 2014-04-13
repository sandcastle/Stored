namespace Stored.Query
{
    public class BinaryFilter : FilterBase
    {
        public BinaryOperator Operator { get; set; }
        public object Value { get; set; }
    }
}