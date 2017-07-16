﻿using System;

namespace Stored.Query
{
    internal static class TypeHelper
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
    }
}
