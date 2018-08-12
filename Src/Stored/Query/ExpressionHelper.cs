using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Stored.Query
{
    public static class ExpressionHelper
    {
        public static string GetName<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertyExpression)
        {
            return GetMemberInfo(propertyExpression).Name;
        }

        public static Type GetPropertyType<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertyExpression)
        {
            var member = GetMemberInfo(propertyExpression) as PropertyInfo;
            if (member == null)
            {
                throw new InvalidOperationException("Property not found.");
            }

            return member.PropertyType;
        }

        static MemberInfo GetMemberInfo<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertyExpression)
        {
            var body = propertyExpression.Body;

            var memberExpression = body as MemberExpression;
            if (memberExpression == null)
            {
                if (body is UnaryExpression unaryExpression)
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
