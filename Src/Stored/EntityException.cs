using System;
using System.Runtime.Serialization;

namespace Stored
{
    public class EntityException : Exception
    {
        public EntityException()
            : this(string.Empty, null) { }

        public EntityException(string message)
            : this(message, null) { }

        public EntityException(string message, Exception innerException)
            : base(message, innerException) { }

        public EntityException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
