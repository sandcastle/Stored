using System;

namespace Stored
{
    public class EntityException : Exception
    {
        public EntityException()
            : this(string.Empty) { }

        public EntityException(string message, Exception innerException = null)
            : base(message, innerException) { }
    }
}
