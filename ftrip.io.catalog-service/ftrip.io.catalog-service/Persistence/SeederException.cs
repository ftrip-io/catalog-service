using System;
using System.Runtime.Serialization;

namespace ftrip.io.catalog_service.Persistence
{
    public class SeederException : Exception, ISerializable
    {
        public SeederException(string message) : base(message)
        {
        }
    }
}
