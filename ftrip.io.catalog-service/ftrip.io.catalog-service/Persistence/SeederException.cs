using System;

namespace ftrip.io.catalog_service.Persistence
{
    public class SeederException:Exception
    {
        public SeederException(string message) : base(message)
        {
        }
    }
}
