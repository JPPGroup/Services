using AutoMapper;
using Jpp.Web.Service.Adapters;
using Jpp.Web.Service.Mappings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.OpenApi.Models;

namespace Jpp.Web.Service
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
            services.AddScoped<IProjectService, ProjectService>();
            services.AddAutoMapper(typeof(ModelToResourceProfile));

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            services.AddMemoryCache();
            services.AddHealthChecks();
            services.AddApplicationInsightsTelemetry();
            services.AddRouting(o =>
            {
                o.LowercaseUrls = true;
                o.AppendTrailingSlash = false;
            });
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JPP Web API", Version = "v1" });
            });

            services.AddAuthentication(IISDefaults.AuthenticationScheme);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "JPP Web API v1");
                c.RoutePrefix = string.Empty;
            });
            app.UseHealthChecks("/healthcheck");
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
