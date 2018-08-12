using System;
using System.Diagnostics;

namespace Stored.Query.Restrictions
{
    [DebuggerDisplay("Not Like {" + nameof(FieldName) + "} : {" + nameof(Value) + "}")]
    public class NotLikeRestriction : IUnaryRestriction
    {
        public NotLikeRestriction(string fieldName, Type fieldType, object value)
        {
            FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            FieldType = fieldType;
            Value = value;
        }

        public string FieldName { get; }
        public Type FieldType { get; }
        public object Value { get; }
    }
}
