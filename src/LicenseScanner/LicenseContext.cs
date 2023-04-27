using Microsoft.EntityFrameworkCore;

namespace LicenseScanner
{
    public class LicenseContext : DbContext
    {

        public LicenseContext(DbContextOptions<LicenseContext> options) : base(options)
        {
        }

        public DbSet<LicenseSession> Sessions { get; set; }
    }
}
