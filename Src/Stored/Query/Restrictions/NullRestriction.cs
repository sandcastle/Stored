using System;
using System.Diagnostics;

namespace Stored.Query.Restrictions
{
    [DebuggerDisplay("Null {" + nameof(FieldName) + "}")]
    public class NullRestriction : IUnaryRestriction
    {
        public NullRestriction(string fieldName)
        {
            FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
        }

        public string FieldName { get; }
    }
}
