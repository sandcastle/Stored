namespace Stored.Query
{
    public interface IFilterBuilder<T>
    {
        IQuery<T> Equal(object value);
        IQuery<T> NotEqual(object value);
    }
}
