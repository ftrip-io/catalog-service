using ftrip.io.catalog_service.Amenities.Domain;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Amenities
{
    public interface IAmenityRepository : IRepository<Amenity, Guid>
    {
        Task<IEnumerable<Amenity>> ReadByIds(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
        Task<IEnumerable<Amenity>> CreateMany(IEnumerable<Amenity> amenities, CancellationToken cancellationToken = default);
    }

    public class AmenityRepository : Repository<Amenity, Guid>, IAmenityRepository
    {
        public AmenityRepository(DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Amenity>> CreateMany(IEnumerable<Amenity> amenities, CancellationToken cancellationToken = default)
        {
            await _entities.AddRangeAsync(amenities, cancellationToken);
            return amenities;
        }

        public override async Task<IEnumerable<Amenity>> Read(CancellationToken cancellationToken = default)
        {
            return await _entities.Include(a => a.Type).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Amenity>> ReadByIds(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            return await _entities.Where(a => ids.Contains(a.Id)).ToListAsync(cancellationToken);
        }
    }
}
