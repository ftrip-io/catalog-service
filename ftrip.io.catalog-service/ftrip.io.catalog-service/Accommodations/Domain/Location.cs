using ftrip.io.framework.Domain;
using System;

namespace ftrip.io.catalog_service.Accommodations.Domain
{
    public class Location : Entity<Guid>
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
