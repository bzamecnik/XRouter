using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectRemoter
{
    [Serializable]
    public class ObjectRemoterException : Exception
    {
        public ObjectRemoterException() { }
        public ObjectRemoterException(string message) : base(message) { }
        public ObjectRemoterException(string message, Exception inner) : base(message, inner) { }
        protected ObjectRemoterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
