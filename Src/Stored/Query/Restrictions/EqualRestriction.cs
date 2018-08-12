using System;
using System.Diagnostics;

namespace Stored.Query.Restrictions
{
    [DebuggerDisplay("Equal {" + nameof(FieldName) + "} : {" + nameof(Value) + "}")]
    public class EqualRestriction : IUnaryRestriction
    {
        public EqualRestriction(string fieldName, Type fieldType, object value)
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
