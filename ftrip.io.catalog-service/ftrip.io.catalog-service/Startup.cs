using ftrip.io.framework.auth;
using ftrip.io.framework.CQRS;
using ftrip.io.framework.ExceptionHandling.Extensions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Installers;
using ftrip.io.framework.Mapping;
using ftrip.io.framework.messaging.Installers;
using ftrip.io.framework.Persistence.Sql.Mariadb;
using ftrip.io.framework.Swagger;
using ftrip.io.framework.Validation;
using ftrip.io.framework.HealthCheck;
using ftrip.io.framework.Secrets;
using ftrip.io.catalog_service.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ftrip.io.catalog_service.Installers;
using System;
using ftrip.io.framework.Tracing;
using ftrip.io.framework.Correlation;
using Serilog;
using ftrip.io.framework.Proxies;
using ftrip.io.framework.Metrics;

namespace ftrip.io.catalog_service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient("booking", client =>
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("BOOKING_SERVICE_URL"))
            );
            InstallerCollection.With(
                new SwaggerInstaller<Startup>(services),
                new AutoMapperInstaller<Startup>(services),
                new FluentValidationInstaller<Startup>(services),
                new GlobalizationInstaller<Startup>(services),
                new EnviromentSecretsManagerInstaller(services),
                new JwtAuthenticationInstaller(services),
                new MariadbInstaller<DatabaseContext>(services),
                new MariadbHealthCheckInstaller(services),
                new CQRSInstaller<Startup>(services),
                new RabbitMQInstaller<Startup>(services, RabbitMQInstallerType.Publisher | RabbitMQInstallerType.Consumer),
                new DependenciesIntaller(services),
                new TracingInstaller(services, (tracingSettings) =>
                {
                    tracingSettings.ApplicationLabel = "catalog";
                    tracingSettings.ApplicationVersion = GetType().Assembly.GetName().Version?.ToString() ?? "unknown";
                    tracingSettings.MachineName = Environment.MachineName;
                }),
                new CorrelationInstaller(services),
                new ProxyGeneratorInstaller(services),
                new MetricsInstaller(services)
            ).Install();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseMetrics();

            app.UseCors(policy => policy
                .WithOrigins(Environment.GetEnvironmentVariable("API_PROXY_URL"))
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCorrelation();

            app.UseFtripioGlobalExceptionHandler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseFtripioSwagger(Configuration.GetSection(nameof(SwaggerUISettings)).Get<SwaggerUISettings>());
            app.UseFtripioHealthCheckUI(Configuration.GetSection(nameof(HealthCheckUISettings)).Get<HealthCheckUISettings>());
        }
    }
}