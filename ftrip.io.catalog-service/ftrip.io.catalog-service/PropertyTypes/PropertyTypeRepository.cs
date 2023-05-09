using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Repository;
using System;
using Microsoft.EntityFrameworkCore;
using ftrip.io.catalog_service.PropertyTypes.Domain;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace ftrip.io.catalog_service.PropertyTypes
{
    public interface IPropertyTypeRepository : IRepository<PropertyType, Guid>
    {
        Task<IEnumerable<PropertyType>> CreateMany(IEnumerable<PropertyType> propertyTypes, CancellationToken cancellationToken = default);
    }

    public class PropertyTypeRepository : Repository<PropertyType, Guid>, IPropertyTypeRepository
    {
        public PropertyTypeRepository(DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PropertyType>> CreateMany(IEnumerable<PropertyType> propertyTypes, CancellationToken cancellationToken = default)
        {
            await _entities.AddRangeAsync(propertyTypes, cancellationToken);
            return propertyTypes;
        }
    }
}
