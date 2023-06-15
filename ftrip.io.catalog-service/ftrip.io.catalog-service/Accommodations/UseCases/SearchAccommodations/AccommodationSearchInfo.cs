using ftrip.io.catalog_service.Accommodations.Domain;
using System;

namespace ftrip.io.catalog_service.Accommodations.UseCases.SearchAccommodations
{
    public class AccommodationSearchInfo
    {
        public Guid AccommodationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public PlaceType PlaceType { get; set; }
        public int BedroomCount { get; set; }
        public int BedCount { get; set; }
        public int BathroomCount { get; set; }
        public Location Location { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal Price { get; set; }
        public bool IsPerGuest { get; set; }
    }
}
