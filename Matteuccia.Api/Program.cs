using System;
using MassTransit.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Extensions.Logging;

namespace Matteuccia.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(host.Services.GetService<IConfiguration>())
                .CreateLogger();

            var loggerFactory = new SerilogLoggerFactory(Log.Logger);
            LogContext.ConfigureCurrentLogContext(loggerFactory);

            try
            {
                Log.Information("Matteuccia API starting up");
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Matteuccia API start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}