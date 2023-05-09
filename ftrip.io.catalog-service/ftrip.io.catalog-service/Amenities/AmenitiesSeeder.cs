using ftrip.io.catalog_service.Amenities.Domain;
using ftrip.io.catalog_service.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.catalog_service.Amenities
{
    public class AmenitiesSeeder
    {
        private readonly IAmenityRepository _amenityRepository;
        private readonly IAmenityTypeRepository _amenityTypeRepository;
        private readonly DbContext _dbContext;

        public AmenitiesSeeder(IAmenityRepository amenityRepository, IAmenityTypeRepository amenityTypeRepository, DbContext dbContext)
        {
            _amenityRepository = amenityRepository;
            _amenityTypeRepository = amenityTypeRepository;
            _dbContext = dbContext;
        }

        public async Task<bool> ShouldSeed()
        {
            return !await _amenityRepository.Query().AnyAsync();
        }

        public async Task Seed()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("amenities");
            if (stream == null)
                throw new SeederException($"Unable to read amenities.json");

            using StreamReader reader = new StreamReader(stream);

            var amenitiesJson = reader.ReadToEnd();
            if (string.IsNullOrEmpty(amenitiesJson))
                throw new SeederException($"No data inside amenities.json");

            var amenities = JsonConvert.DeserializeObject<List<Amenity>>(amenitiesJson);

            var amenityTypes = amenities.Select(a => a.Type.Name).Distinct().Select(n => new AmenityType { Name = n }).ToList();

            amenityTypes = (await _amenityTypeRepository.CreateMany(amenityTypes, CancellationToken.None)).ToList();

            amenities.ForEach(a =>
            {
                var amenityType = amenityTypes.FirstOrDefault(at => at.Name == a.Type.Name);
                if (amenityType != null) a.AmenityTypeId = amenityType.Id;
                a.Type = null;
            });

            await _amenityRepository.CreateMany(amenities, CancellationToken.None);

            await _dbContext.SaveChangesAsync();
        }
    }

    public static class AmenitiesSeederExtensions
    {
        public static IHost SeedAmenities(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<IAmenityRepository>>();
            var seeder = new AmenitiesSeeder(
                services.GetRequiredService<IAmenityRepository>(),
                services.GetRequiredService<IAmenityTypeRepository>(),
                services.GetRequiredService<DbContext>()
            );

            if (seeder.ShouldSeed().Result)
            {
                logger.LogInformation("Seeding amenities");
                seeder.Seed().Wait();
            }
            else
            {
                logger.LogInformation("Skipped seeding amenities");
            }
            return host;
        }
    }
}
