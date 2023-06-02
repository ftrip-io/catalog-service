using ftrip.io.catalog_service.Accommodations;
using ftrip.io.catalog_service.Amenities;
using ftrip.io.catalog_service.PropertyTypes;
using ftrip.io.framework.Installers;
using ftrip.io.framework.Proxies;
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
            _services.AddProxiedScoped<IAccommodationRepository, AccommodationRepository>();
            _services.AddProxiedScoped<IAmenityRepository, AmenityRepository>();
            _services.AddProxiedScoped<IAmenityTypeRepository, AmenityTypeRepository>();
            _services.AddProxiedScoped<IPropertyTypeRepository, PropertyTypeRepository>();
            _services.AddProxiedScoped<IBookingServiceClient, BookingServiceClient>();
        }
    }
}
