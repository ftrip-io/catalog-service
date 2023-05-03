using ftrip.io.catalog_service.Amenities.Domain;
using ftrip.io.catalog_service.PropertyTypes.Domain;
using ftrip.io.framework.Domain;
using System;
using System.Collections.Generic;

namespace ftrip.io.catalog_service.Accommodations.Domain
{
    public class Accommodation : Entity<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public PlaceType PlaceType { get; set; }
        public bool IsDecicatedForGuests { get; set; }
        public int MinGuests { get; set; }
        public int MaxGuests { get; set; }
        public int BedroomCount { get; set; }
        public int BedCount { get; set; }
        public int BathroomCount { get; set; }
        public int NoticePeriod { get; set; }
        public int BookingAdvancePeriod { get; set; }
        public int CheckInFrom { get; set; }
        public int CheckInTo { get; set; }
        public int BookBeforeTime { get; set; }
        public int MinNights { get; set; }
        public int MaxNights { get; set; }
        public decimal Price { get; set; }
        public bool IsPerGuest { get; set; }
        public Guid HostId { get; set; }
        public string HouseRules { get; set; }
        public Guid LocationId { get; set; }
        public Guid PropertyTypeId { get; set; }

        public virtual Location Location { get; set; }
        public virtual PropertyType PropertyType { get; set; }
        public virtual ICollection<AccommodationAmenity> Amenities { get; set; }
        public virtual ICollection<Availability> Availabilities { get; set; }
        public virtual ICollection<PriceDiff> PriceDiffs { get; set; }
    }

    public enum PlaceType
    {
        ENTIRE_PLACE, PRIVATE_ROOM, SHARED_ROOM
    }

    public class AccommodationAmenity : Entity<Guid>
    {
        public Guid AmenityId { get; set; }
        public Guid AccommodationId { get; set; }
        public bool IsPresent { get; set; }

        public virtual Amenity Amenity { get; set; }
    }
}
