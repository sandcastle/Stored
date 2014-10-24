using System;

namespace Stored.Query
{
    internal static class TypeHelper
    {
        public static object GetUnderlyingValue(object value)
        {
            var type = value.GetType();
            if (type.IsEnum)
            {
                var underlyingType = Enum.GetUnderlyingType(type);
                return Convert.ChangeType(value, underlyingType);
            }

            return value;
        }
    }
}
