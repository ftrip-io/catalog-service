using ftrip.io.catalog_service.Accommodations.Domain;
using ftrip.io.catalog_service.Amenities.Domain;
using ftrip.io.catalog_service.PropertyTypes.Domain;
using ftrip.io.framework.Contexts;
using ftrip.io.framework.Persistence.Sql.Database;
using Microsoft.EntityFrameworkCore;

namespace ftrip.io.catalog_service.Persistence
{
    public class DatabaseContext : DatabaseContextBase<DatabaseContext>
    {
        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<AmenityType> AmenityTypes { get; set; }
        public DbSet<AccommodationAmenity> AccommodationAmenities { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options, CurrentUserContext currentUserContext) :
            base(options, currentUserContext)
        {
        }
    }
}