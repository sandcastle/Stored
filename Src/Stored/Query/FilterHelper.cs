using System;

namespace Stored.Query
{
    internal static class FilterHelper
    {
        public static Func<T, bool> Filter<T>(FilterBase filter)
        {
            if (filter is BinaryFilter)
            {
                return BinaryFilter<T>(filter as BinaryFilter);
            }

            throw new NotSupportedException("");
        }

        public static Func<T, bool> BinaryFilter<T>(BinaryFilter filter)
        {
            var property = typeof(T).GetProperty(filter.FieldName);
            if (property.CanRead == false)
            {
                throw new Exception("Property cannot be read.");
            }

            if (filter.Operator == BinaryOperator.Equal)
            {
                return (x) => property.GetValue(x, null).Equals(filter.Value);
            }

            if (filter.Operator == BinaryOperator.NotEqual)
            {
                return (x) => property.GetValue(x, null).Equals(filter.Value) == false;
            }

            throw new NotSupportedException(String.Format("Operator {0} is not supported.", filter.Operator));
        }
    }
}