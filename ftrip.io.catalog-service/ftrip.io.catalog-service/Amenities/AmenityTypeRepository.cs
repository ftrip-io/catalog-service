using ftrip.io.catalog_service.Amenities.Domain;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Amenities
{
    public interface IAmenityTypeRepository : IRepository<AmenityType, Guid>
    {
        Task<IEnumerable<AmenityType>> CreateMany(IEnumerable<AmenityType> amenityTypes, CancellationToken cancellationToken = default);
    }

    public class AmenityTypeRepository : Repository<AmenityType, Guid>, IAmenityTypeRepository
    {
        public AmenityTypeRepository(DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AmenityType>> CreateMany(IEnumerable<AmenityType> amenityTypes, CancellationToken cancellationToken = default)
        {
            await _entities.AddRangeAsync(amenityTypes, cancellationToken);
            return amenityTypes;
        }
    }
}
