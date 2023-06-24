using HueSensorExporter.service.Options;
using HueSensorExporter.service.Services;
using Microsoft.AspNetCore.Builder;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Serilog;

namespace HueSensorExporter.service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog((context, services, logger) =>
            {
                logger.ReadFrom.Services(services).ReadFrom.Configuration(context.Configuration);
            });

            builder.Services.Configure<HueApiOptions>(builder.Configuration.GetSection(HueApiOptions.ConfigSection));
            builder.Services.AddSingleton<LocalHueApiService>();
            builder.Services.AddSingleton<HueMetricService>();
            builder.Services.AddSingleton<HueManagementService>();
            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resourceBuilder => resourceBuilder.AddService("HueSensorExporter"))
                .WithMetrics(providerBuilder =>
                {
                    providerBuilder.AddMeter(HueMetricService.MeterName);
                    providerBuilder.AddPrometheusExporter();
                });
            builder.Services.AddHostedService<Worker>();

            var app = builder.Build();

            app.MapPrometheusScrapingEndpoint();

            app.Run();
        }
    }
}