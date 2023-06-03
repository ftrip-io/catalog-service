using System;
using System.Runtime.Serialization;

namespace ftrip.io.catalog_service.Persistence
{
    [Serializable]
    public class SeederException : Exception
    {
        public SeederException(string message) : base(message) { }

        protected SeederException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
