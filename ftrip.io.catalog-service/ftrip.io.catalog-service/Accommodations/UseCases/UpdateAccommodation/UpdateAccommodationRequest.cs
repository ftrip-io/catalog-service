using ftrip.io.catalog_service.Accommodations.Domain;
using MediatR;
using System;
using System.Collections.Generic;

namespace ftrip.io.catalog_service.Accommodations.UseCases.UpdateAccommodation
{
    public abstract class PartialAccommodationUpdateRequest: IRequest<Accommodation>
    {
        public Guid Id { get; set; }
    }

    public class UpdateAccommodationRequest : PartialAccommodationUpdateRequest
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
        public int CheckInFrom { get; set; }
        public int CheckInTo { get; set; }
        public int BookBeforeTime { get; set; }
        public int MinNights { get; set; }
        public int MaxNights { get; set; }
        public string HouseRules { get; set; }
        public Guid PropertyTypeId { get; set; }
    }

    public class UpdateAccommodationLocationRequest : PartialAccommodationUpdateRequest
    {
        public LocationUpdate Location { get; set; }
    }

    public class UpdateAccommodationAmenitiesRequest : PartialAccommodationUpdateRequest
    {
        public ICollection<AccommodationAmenityUpdate> Amenities { get; set; }
    }

    public class UpdateAccommodationAvailabilitiesRequest : PartialAccommodationUpdateRequest
    {
        public int BookingAdvancePeriod { get; set; }
        public ICollection<AvailabilityUpdate> Availabilities { get; set; }
    }

    public class UpdateAccommodationPricingRequest : PartialAccommodationUpdateRequest
    {
        public decimal Price { get; set; }
        public bool IsPerGuest { get; set; }
        public ICollection<PriceDiffUpdate> PriceDiffs { get; set; }
    }

    public class AccommodationAmenityUpdate
    {
        public Guid Id { get; set; }
        public Guid AmenityId { get; set; }
        public bool IsPresent { get; set; }
    }

    public class LocationUpdate
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

    public class AvailabilityUpdate
    {
        public Guid Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class PriceDiffUpdate
    {
        public Guid Id { get; set; }
        public string When { get; set; }
        public float Percentage { get; set; }
    }
}
