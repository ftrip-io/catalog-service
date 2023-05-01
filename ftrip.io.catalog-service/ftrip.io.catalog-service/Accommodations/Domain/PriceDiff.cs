using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.catalog_service.Accommodations.Domain
{
    public class PriceDiff : Entity<Guid>
    {
        public string When { get; set; } //?
        public float Percentage { get; set; }
        public Guid AccommodationId { get; set; }
    }
}
