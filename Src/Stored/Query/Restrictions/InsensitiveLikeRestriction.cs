using System;
using System.Diagnostics;

namespace Stored.Query.Restrictions
{
    [DebuggerDisplay("Insensitive Like {" + nameof(FieldName) + "} : {" + nameof(Value) + "}")]
    public class InsensitiveLikeRestriction : IUnaryRestriction
    {
        // https://www.postgresql.org/docs/9.6/static/functions-matching.html - ilike

        public InsensitiveLikeRestriction(string fieldName, Type fieldType, object value)
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
