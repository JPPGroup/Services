using System;
using Jpp.Files.Api.TelemetryFilters;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace Jpp.Files.Api
{
    public class Startup
    {
        private readonly ILogger _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {            
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMvc()
                    .AddJsonOptions(opts => { opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

                services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Configuration["UPLOADS_ROOT"]));

                services.AddCors(options =>
                {
                    options.AddPolicy("AllowAllOrigins", GenerateCorsPolicy());
                });

                services.AddSwaggerGen(c =>
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
                });

                services.AddHealthChecks();
                services.AddRouting(o =>
                {
                    o.LowercaseUrls = true;
                    o.AppendTrailingSlash = false;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                throw;
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseHsts();
                }

                var telemetryBuilder = app.ApplicationServices.GetService<TelemetryConfiguration>().TelemetryProcessorChainBuilder;
                telemetryBuilder.Use(next => new HealthCheckFilter(next));
                telemetryBuilder.Build();

                app.UseHttpsRedirection();
                app.UseCors("AllowAllOrigins");

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Files API V1");
                    c.RoutePrefix = string.Empty;
                });
                app.UseHealthChecks("/healthcheck");
                app.UseMvc();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                throw;
            }
        }

        private static CorsPolicy GenerateCorsPolicy()
        {
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin();
            corsBuilder.AllowCredentials();
            return corsBuilder.Build();
        }
    }
}
