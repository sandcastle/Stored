using System;
using System.Linq;

namespace Stored.Query
{
    public static class TypeHelper
    {
        static readonly Type _boolType = typeof (bool);

        public static object GetUnderlyingValue(object value)
        {
            var type = value.GetType();
            if (type.IsEnum)
            {
                var underlyingType = Enum.GetUnderlyingType(type);
                return Convert.ChangeType(value, underlyingType);
            }

            if (type == _boolType)
            {
                return value.ToString().ToLower();
            }

            return value;
        }

        internal static string ValueToStrings(object value)
        {
            if (value == null)
            {
                return "null";
            }

            var type = value.GetType();
            if (type.IsPrimitive || _callToStringTypes.Any(t => t.IsAssignableFrom(type)))
            {
                return value.ToString();
            }

            // TODO: Support custom conversions

            return value.ToString();
        }

        static readonly Type[] _callToStringTypes = {
            typeof(DateTime),
            typeof(string)
        };
    }
}
