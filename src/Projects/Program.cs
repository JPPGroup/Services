using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Jpp.Web.Service
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => {
                    var env = hostingContext.HostingEnvironment;
                    config
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

                    config.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
