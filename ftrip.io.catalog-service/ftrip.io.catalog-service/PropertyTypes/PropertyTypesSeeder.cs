using ftrip.io.catalog_service.Persistence;
using ftrip.io.catalog_service.PropertyTypes.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.PropertyTypes
{
    public class PropertyTypesSeeder
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly DbContext _dbc;

        public CancellationToken CancellationToken { get; private set; }

        public PropertyTypesSeeder(IPropertyTypeRepository propertyTypeRepository, DbContext dbc)
        {
            _propertyTypeRepository = propertyTypeRepository;
            _dbc = dbc;
        }

        public async Task<bool> ShouldSeed()
        {
            return !await _propertyTypeRepository.Query().AnyAsync();
        }

        public async Task Seed()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("propertyTypes");
            if (stream == null)
                throw new SeederException($"Unable to read propertyTypes.json");

            using StreamReader reader = new StreamReader(stream);

            var propertyTypesJson = reader.ReadToEnd();
            if (string.IsNullOrEmpty(propertyTypesJson))
                throw new SeederException($"No data inside propertyTypes.json");

            var propertyTypes = JsonConvert.DeserializeObject<List<PropertyType>>(propertyTypesJson);

            await _propertyTypeRepository.CreateMany(propertyTypes, CancellationToken.None);

            await _dbc.SaveChangesAsync();
        }
    }

    public static class PropertyTypesSeederExtensions
    {
        public static IHost SeedPropertyTypes(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<IPropertyTypeRepository>>();
            var seeder = new PropertyTypesSeeder(
                services.GetRequiredService<IPropertyTypeRepository>(),
                services.GetRequiredService<DbContext>()
            );

            if (seeder.ShouldSeed().Result)
            {
                logger.LogInformation("Seeding property types");
                seeder.Seed().Wait();
            }
            else
            {
                logger.LogInformation("Skipped seeding property types");
            }

            return host;
        }
    }
}
