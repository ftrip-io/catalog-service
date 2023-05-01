using ftrip.io.catalog_service.Accommodations.Domain;
using ftrip.io.framework.Mapping;
using MediatR;
using System;
using System.Collections.Generic;

namespace ftrip.io.catalog_service.Accommodations.UseCases.CreateAccommodation
{
    [Mappable(Destination = typeof(Accommodation))]
    public class CreateAccommodationRequest : IRequest<Accommodation>
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
        public Guid PropertyTypeId { get; set; }
        public CreateLocationRequest Location { get; set; }
        public ICollection<CreateAccommodationAmenityRequest> Amenities { get; set; }
    }

    [Mappable(Destination = typeof(AccommodationAmenity))]
    public class CreateAccommodationAmenityRequest
    {
        public Guid AmenityId { get; set; }
        public bool IsPresent { get; set; }
    }

    [Mappable(Destination = typeof(Location))]
    public class CreateLocationRequest
    {
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public string Apt { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}
