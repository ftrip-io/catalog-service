using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.catalog_service.Accommodations.Domain
{
    public class Amenity : Entity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public Guid AmenityTypeId { get; set; }

        public virtual AmenityType Type { get; set; }
    }

    public class AmenityType : Entity<Guid>
    {
        public string Name { get; set; }
    }

    public class AccommodationAmenity : Entity<Guid>
    {
        public Guid AmenityId { get; set; }
        public Guid AccommodationId { get; set; }
        public bool IsPresent { get; set; }

        public virtual Amenity Amenity { get; set; }
    }
}
