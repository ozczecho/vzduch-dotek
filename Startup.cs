using Lamar;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VzduchDotek.Net.TcpMessaging;

namespace VzduchDotek.Net
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureContainer(ServiceRegistry services)
        {
            services.Configure<AirTouchOptions>(Configuration.GetSection(AirTouchOptions.AirTouch));
            services.AddHealthChecks();
            services.Scan(s =>
            {
                s.TheCallingAssembly();
                s.WithDefaultConventions();
            });

            services.For<ITcpClient>().Use<TcpClientImpl>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseHealthChecks("/healthcheck");
            app.UseExceptionHandler(new ExceptionHandlerOptions { AllowStatusCode404Response = true });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
