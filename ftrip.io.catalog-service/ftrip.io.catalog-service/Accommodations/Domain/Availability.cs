using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.catalog_service.Accommodations.Domain
{
    public class Availability : Entity<Guid>
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsAvailable { get; set; }
        public Guid AccommodationId { get; set; }
    }
}
