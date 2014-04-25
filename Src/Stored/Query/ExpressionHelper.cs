using System;
using System.Linq.Expressions;

namespace Stored.Query
{
    internal static class ExpressionHelper
    {
        public static string GetName<T>(Expression<Func<T, object>> propertyExpression)
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
                throw new Exception(String.Format("Unknown property type: '{0}'.", body));
            }

            return memberExpression.Member.Name;
        }
    }
}