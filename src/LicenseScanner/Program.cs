
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace LicenseScanner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            // Add services to the container.
            builder.Services.AddDbContext<LicenseContext>(options =>
            {
#if DEBUG
                options.UseSqlServer(connectionString, x => x.MigrationsAssembly("LicenseScanner"));
#else
                options.UseNpgsql(connectionString, x => x.MigrationsAssembly("LicenseScannerPostgresqlMigrations"));                                                
#endif                
            });
            builder.Services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); ;
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}