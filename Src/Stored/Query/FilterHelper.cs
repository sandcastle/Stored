using System;

namespace Stored.Query
{
    internal static class FilterHelper
    {
        // public static Func<T, bool> Filter<T>(FilterBase filter)
        // {
        //     if (filter is BinaryFilter binaryFilter)
        //     {
        //         return BinaryFilter<T>(binaryFilter);
        //     }
        //
        //     throw new NotSupportedException($"Filter type '{filter.GetType()}' not supported");
        // }

        // public static Func<T, bool> BinaryFilter<T>(BinaryFilter filter)
        // {
        //     var property = typeof(T).GetProperty(filter.FieldName);
        //     if (property == null)
        //     {
        //         throw new InvalidOperationException("Property not found.");
        //     }
        //
        //     if (property.CanRead == false)
        //     {
        //         throw new Exception("Property cannot be read.");
        //     }
        //
        //     if (filter.Operator == BinaryOperator.Equal)
        //     {
        //         return x => TypeHelper.GetUnderlyingValue(property.GetValue(x, null)).Equals(filter.Value);
        //     }
        //
        //     if (filter.Operator == BinaryOperator.NotEqual)
        //     {
        //         return x => TypeHelper.GetUnderlyingValue(property.GetValue(x, null)).Equals(filter.Value) == false;
        //     }
        //
        //     throw new NotSupportedException($"Operator '{filter.Operator}' is not supported.");
        // }
    }
}
