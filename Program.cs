using System;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;

namespace VzduchDotek.Net
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration() 
                .ReadFrom.Configuration(Configuration)
                //.MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            try 
            {
                Log.ForContext<Program>().Information("Starting host");
                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception ex)
            {
                Log.ForContext<Program>().Fatal(ex, "Host has terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables(prefix: "DOTEK_")
            .Build();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables(prefix: "DOTEK_");
                })
                .UseLamar()
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                { 
                    var port = Configuration["vzduchPort"] ?? "80";
                     
                    Log.ForContext<Program>().Information($"Airtouch panel host + port is [{Configuration["airTouch:localHost"]}:{Configuration["airTouch:localPort"]}]");
                    Log.ForContext<Program>().Information($"LogLevel [{Configuration["Serilog:MinimumLevel:Default"]}]");

                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls($"http://*:{port}");
                    webBuilder.UseKestrel();
                    webBuilder.ConfigureServices(services =>
                    {
                        // This is important, the call to AddControllers()
                        // cannot be made before the usage of ConfigureWebHostDefaults
                        services.AddControllers();
                    });
                });
    }
}
