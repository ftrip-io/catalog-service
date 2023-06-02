using System;

namespace ftrip.io.catalog_service.contracts.Accommodations
{
    public class AccommodationOccupancy
    {
        public Guid AccomodationId { get; set; }
        public DatePeriod DatePeriod { get; set; }
        public decimal Price { get; set; }
        public int Guests { get; set; }
        public AccommodationOccupancyType OccupancyType { get; set; }
    }

    public class DatePeriod
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }

    public enum AccommodationOccupancyType
    {
        Reservation,
        Request
    }
}
