using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Repository;
using System;
using Microsoft.EntityFrameworkCore;
using ftrip.io.catalog_service.PropertyTypes.Domain;

namespace ftrip.io.catalog_service.PropertyTypes
{
    public interface IPropertyTypeRepository : IRepository<PropertyType, Guid>
    {
    }

    public class PropertyTypeRepository : Repository<PropertyType, Guid>, IPropertyTypeRepository
    {
        public PropertyTypeRepository(DbContext context) : base(context)
        {
        }
    }
}
