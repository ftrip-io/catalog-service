using ftrip.io.catalog_service.Accommodations;
using ftrip.io.catalog_service.Amenities;
using ftrip.io.catalog_service.PropertyTypes;
using ftrip.io.framework.Installers;
using Microsoft.Extensions.DependencyInjection;

namespace ftrip.io.catalog_service.Installers
{
    public class DependenciesIntaller : IInstaller
    {
        private readonly IServiceCollection _services;

        public DependenciesIntaller(IServiceCollection services)
        {
            _services = services;
        }

        public void Install()
        {
            _services.AddScoped<IAccommodationRepository, AccommodationRepository>();
            _services.AddScoped<IAmenityRepository, AmenityRepository>();
            _services.AddScoped<IAmenityTypeRepository, AmenityTypeRepository>();
            _services.AddScoped<IPropertyTypeRepository, PropertyTypeRepository>();
        }
    }
}
