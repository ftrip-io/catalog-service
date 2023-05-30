using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.catalog_service.contracts.Accommodations.Events
{
    public class AccommodationCreatedEvent : Event<string>
    {
        public Guid AccommodationId { get; set; }
        public Guid HostId { get; set; }

        public AccommodationCreatedEvent()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}