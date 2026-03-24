using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TwoDrive.Persistence.Models;

namespace TwoDrive.Persistence;

internal class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Build configuration from the web app directory
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "TwoDrive"))
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        optionsBuilder.UseSeeding((dbContext, databaseUpdated) =>
        {
            SeedData(dbContext);
        });

        return new AppDbContext(optionsBuilder.Options);
    }

    private static void SeedData(DbContext dbContext)
    {
        // Seed Folders
        if (!dbContext.Set<FolderModel>().Any())
        {
            var casFolder = new FolderModel
            {
                Id = Guid.NewGuid(),
                Name = "CAS",
                Path = "/",
                OwnerId = Guid.Empty,
                CreatedAt = new DateTime(2025, 4, 30),
                UpdatedAt = new DateTime(2025, 4, 30)
            };
            dbContext.Add(casFolder);
            dbContext.SaveChanges();
        }

        // Seed Files
        if (!dbContext.Set<FileModel>().Any())
        {
            var rootFolderId = dbContext.Set<FolderModel>()
                .FirstOrDefault(f => f.Name == "CAS")?.Id ?? Guid.Empty;

            var files = new[]
            {
                new FileModel
                {
                    Id = Guid.NewGuid(),
                    Name = "CoasterAndBargelLoading",
                    Extension = "docx",
                    Path = "/",
                    MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    FolderId = rootFolderId,
                    OwnerId = Guid.Empty,
                    SizeBytes = 0,
                    StorageKey = string.Empty,
                    Checksum = string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new FileModel
                {
                    Id = Guid.NewGuid(),
                    Name = "RevenueByServices",
                    Extension = "xlsx",
                    Path = "/",
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FolderId = rootFolderId,
                    OwnerId = Guid.Empty,
                    SizeBytes = 0,
                    StorageKey = string.Empty,
                    Checksum = string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new FileModel
                {
                    Id = Guid.NewGuid(),
                    Name = "RevenueByServices2016",
                    Extension = "xlsx",
                    Path = "/",
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FolderId = rootFolderId,
                    OwnerId = Guid.Empty,
                    SizeBytes = 0,
                    StorageKey = string.Empty,
                    Checksum = string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new FileModel
                {
                    Id = Guid.NewGuid(),
                    Name = "RevenueByServices2017",
                    Extension = "xlsx",
                    Path = "/",
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FolderId = rootFolderId,
                    OwnerId = Guid.Empty,
                    SizeBytes = 0,
                    StorageKey = string.Empty,
                    Checksum = string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            foreach (var file in files)
            {
                dbContext.Add(file);
            }
            dbContext.SaveChanges();
        }
    }
}
