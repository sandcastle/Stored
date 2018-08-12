using System;
using System.Diagnostics;

namespace Stored.Query.Restrictions
{
    [DebuggerDisplay("Not Null {" + nameof(FieldName) + "}")]
    public class NotNullRestriction : IUnaryRestriction
    {
        public NotNullRestriction(string fieldName)
        {
            FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
        }

        public string FieldName { get; }
    }
}
