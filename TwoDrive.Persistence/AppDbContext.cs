using Microsoft.EntityFrameworkCore;
using TwoDrive.Persistence.Models;

namespace TwoDrive.Persistence
{
    internal class AppDbContext : DbContext
    {
        public DbSet<FolderPersistence> Folders => Set<FolderPersistence>();
        public DbSet<FilePersistence> Files => Set<FilePersistence>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
