using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Repository;
using ftrip.io.catalog_service.Accommodations.Domain;
using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;

namespace ftrip.io.catalog_service.Accommodations
{
    public interface IAccommodationRepository : IRepository<Accommodation, Guid>
    {
        Task<Accommodation> ReadById(Guid id, CancellationToken cancellationToken = default);
    }

    public class AccommodationRepository : Repository<Accommodation, Guid>, IAccommodationRepository
    {
        public AccommodationRepository(DbContext context) : base(context)
        {
        }

        public async Task<Accommodation> ReadById(Guid id, CancellationToken cancellationToken = default)
        {
            return await _entities
                .Include(a => a.PropertyType)
                .Include(a => a.Amenities).ThenInclude(aa => aa.Amenity)
                .Include(a => a.Location)
                .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        }
    }
}
