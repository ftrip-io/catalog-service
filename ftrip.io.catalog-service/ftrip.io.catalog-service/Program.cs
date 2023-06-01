using ftrip.io.catalog_service.Amenities;
using ftrip.io.catalog_service.Persistence;
using ftrip.io.catalog_service.PropertyTypes;
using ftrip.io.framework.Logging;
using ftrip.io.framework.Persistence.Sql.Migrations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace ftrip.io.catalog_service
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .MigrateDbContext<DatabaseContext>()
                .SeedAmenities().SeedPropertyTypes()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilogLogging((hostingContext) =>
                {
                    return new LoggingOptions()
                    {
                        ApplicationName = hostingContext.HostingEnvironment.ApplicationName,
                        ApplicationLabel = "catalog",
                        ClientIdAttribute = "X-Forwarded-For",
                        GrafanaLokiUrl = Environment.GetEnvironmentVariable("GRAFANA_LOKI_URL")
                    };
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
