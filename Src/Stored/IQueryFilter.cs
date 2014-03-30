namespace Stored
{
    public interface IQueryFilter
    {
        string Name { get; }
        QueryOperator Operator { get; }
        object Value { get; }
    }
}