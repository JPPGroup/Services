using Microsoft.EntityFrameworkCore;

namespace Jpp.Projects.MailAI
{
    public class MailDbContext : DbContext
    {
        public DbSet<MailClassification> Classifications { get; set; }

        public MailDbContext(DbContextOptions<MailDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("MailAI");
        }
    }
}
