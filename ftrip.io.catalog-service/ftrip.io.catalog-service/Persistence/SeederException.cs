using System;

namespace ftrip.io.catalog_service.Persistence
{
    [Serializable]
    public class SeederException : Exception
    {
        public SeederException(string message) : base(message)
        {
        }
    }
}
