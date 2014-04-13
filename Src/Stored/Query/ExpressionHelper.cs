using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Stored.Query
{
    internal static class ExpressionHelper
    {
        public static MemberInfo GetMember(this Expression expression)
        {
            var memberExpression = expression.GetMemberExpression();
            return memberExpression.Member.ToMember();
        }

        public static MemberExpression GetMemberExpression(this Expression expression)
        {
            var memberExpression = expression;
            var lambdaExpression = expression as LambdaExpression;
            if (lambdaExpression != null)
            {
                memberExpression = lambdaExpression.Body;
            }

            if (memberExpression.NodeType == ExpressionType.MemberAccess)
            {
                return memberExpression as MemberExpression;
            }

            throw new ArgumentException("Not a member access", "expression");
        }

        public static MemberInfo ToMember(this MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                throw new NullReferenceException("Cannot create member from null.");
            }

            if (memberInfo is PropertyInfo)
            {
                return memberInfo;
            }

            throw new InvalidOperationException(String.Format("Cannot convert MemberInfo '{0}' to Member.", memberInfo.Name));
        }
    }
}