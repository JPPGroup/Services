using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jpp.Files
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {            
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            /*.AddJsonOptions(opts => { opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);*/

            services.AddCors(options => { options.AddPolicy("AllowAllOrigins", GenerateCorsPolicy()); });
            services.AddSingleton<FileManager>();

            /*services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Files API",
                    Version = "v1",
                    Contact = new Contact
                    {
                        Email = "software@jppuk.net"
                    }
                });
            });*/

            services.AddHealthChecks();
            services.AddRouting(o =>
            {
                o.LowercaseUrls = true;
                o.AppendTrailingSlash = false;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();

            /*var telemetryBuilder = app.ApplicationServices.GetService<TelemetryConfiguration>().TelemetryProcessorChainBuilder;
            telemetryBuilder.Use(next => new HealthCheckFilter(next));
            telemetryBuilder.Build();*/

            //app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins");

            /*app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Files API V1");
                c.RoutePrefix = string.Empty;
            });*/
            app.UseHealthChecks("/healthcheck");

            app.UseEndpoints(endpoints =>
            {
                // Mapping of endpoints goes here:
                endpoints.MapControllers();
            });
        }

        private static CorsPolicy GenerateCorsPolicy()
        {
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin();
            return corsBuilder.Build();
        }
    }
}
