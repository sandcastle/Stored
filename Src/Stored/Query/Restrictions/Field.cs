using System;
using System.Linq.Expressions;

namespace Stored.Query.Restrictions
{
    public static class Field
    {
        public static FieldDescriptor AsField<TModel, TProperty>(this Expression<Func<TModel, TProperty>> expression) => new FieldDescriptor
        {
            Name = ExpressionHelper.GetName(expression),
            Type = ExpressionHelper.GetPropertyType(expression)
        };
    }
}
