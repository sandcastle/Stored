using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Stored.Query
{
    public static class ExpressionHelper
    {
        public static string GetName<T>(Expression<Func<T, object>> propertyExpression)
        {
            var member = GetMemberInfo(propertyExpression);

            return member.Name;
        }

        public static Type GetPropertyType<T>(Expression<Func<T, object>> propertyExpression)
        {
            var member = GetMemberInfo(propertyExpression) as PropertyInfo;
            if (member == null)
            {
                throw new InvalidOperationException("Property not found.");
            }

            return member.PropertyType;
        }

        private static MemberInfo GetMemberInfo<T>(Expression<Func<T, object>> propertyExpression)
        {
            var body = propertyExpression.Body;

            var memberExpression = body as MemberExpression;
            if (memberExpression == null)
            {
                var unaryExpression = body as UnaryExpression;
                if (unaryExpression != null)
                {
                    memberExpression = unaryExpression.Operand as MemberExpression;
                }
            }

            if (memberExpression == null)
            {
                throw new Exception($"Unknown property type: '{body}'.");
            }

            return memberExpression.Member;
        }
    }
}
