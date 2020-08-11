using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using MassTransit.Definition;
using Matteuccia.Components;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Matteuccia.Workers
{
    class Program
    {
        static TelemetryClient _telemetryClient;
        static DependencyTrackingTelemetryModule _module;
        
        static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || ((IList) args).Contains("--console"));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                        config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    _module = new DependencyTrackingTelemetryModule();
                    _module.IncludeDiagnosticSourceActivities.Add("MassTransit");

                    TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
                    configuration.InstrumentationKey = "168f4189-bd3e-40e6-a453-0d9751b6ac23";
                    configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

                    _telemetryClient = new TelemetryClient(configuration);

                    _module.Initialize(configuration);

                    services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
                    services.AddMassTransit(cfg =>
                    {
                        cfg.AddConsumer<SubmitOrderConsumer>();
                        cfg.UsingAzureServiceBus(ConfigureBus);
                    });

                    services.AddMassTransitHostedService();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddSerilog(dispose: true);
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                });

            if (isService)
                await builder.UseWindowsService().Build().RunAsync();
            else
                await builder.RunConsoleAsync();

            Log.CloseAndFlush();
        }

        static void ConfigureBus(IBusRegistrationContext context, IServiceBusBusFactoryConfigurator configurator)
        {
            configurator.Host("Endpoint=sb://matteuccia.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=d+9gNw9G2H6uEDBeG+NkNYmcN5rJ0g9K9e0HtU0mdRk=");
            configurator.ConfigureEndpoints(context);
        }
    }
}