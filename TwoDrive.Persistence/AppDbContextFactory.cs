using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TwoDrive.Core;
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


    /// <summary>
    /// NOTE: This seed will fail if data base is not re-created
    /// </summary>
    /// <param name="dbContext"></param>
    private static void SeedData(DbContext dbContext)
    {
        // Seed Folders
        if (!dbContext.Set<FolderPersistence>().Any())
        {
            var casFolder = new FolderPersistence
            {
                Id = Guid.NewGuid(),
                Name = "CAS",
                Path = "/",
                CreatedByUserId = CoreConstants.DEFAULT_USER_ID,
                CreatedByUserNameSnapshot = "Megan Bowel",
                UpdatedByUserId = CoreConstants.DEFAULT_USER_ID,
                UpdatedByUserNameSnapshot = "Megan Bowel",
                CreatedAt = new DateTime(2025, 4, 30),
                UpdatedAt = new DateTime(2025, 4, 30)
            };
            dbContext.Add(casFolder);
            dbContext.SaveChanges();
        }

        // Seed Files
        if (!dbContext.Set<FilePersistence>().Any())
        {
            var rootFolderId = dbContext.Set<FolderPersistence>()
                .FirstOrDefault(f => f.Name == "CAS")?.Id ?? Guid.Empty;

            var files = new[]
            {
                new FilePersistence
                {
                    Id = Guid.NewGuid(),
                    Name = "CoasterAndBargelLoading",
                    Extension = "docx",
                    Path = "/",
                    MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    FolderId = rootFolderId,
                    CreatedByUserId = CoreConstants.DEFAULT_USER_ID,
                    CreatedByUserNameSnapshot = "Megan Bowel",
                    UpdatedByUserId = CoreConstants.DEFAULT_USER_ID,
                    UpdatedByUserNameSnapshot = "Megan Bowel",
                    SizeBytes = 0,
                    StorageKey = string.Empty,
                    Checksum = string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new FilePersistence
                {
                    Id = Guid.NewGuid(),
                    Name = "RevenueByServices",
                    Extension = "xlsx",
                    Path = "/",
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FolderId = rootFolderId,
                    CreatedByUserId = CoreConstants.DEFAULT_USER_ID,
                    CreatedByUserNameSnapshot = "Megan Bowel",
                    UpdatedByUserId = CoreConstants.DEFAULT_USER_ID,
                    UpdatedByUserNameSnapshot = "Megan Bowel",
                    SizeBytes = 0,
                    StorageKey = string.Empty,
                    Checksum = string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new FilePersistence
                {
                    Id = Guid.NewGuid(),
                    Name = "RevenueByServices2016",
                    Extension = "xlsx",
                    Path = "/",
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FolderId = rootFolderId,
                    CreatedByUserId = CoreConstants.DEFAULT_USER_ID,
                    CreatedByUserNameSnapshot = "Megan Bowel",
                    UpdatedByUserId = CoreConstants.DEFAULT_USER_ID,
                    UpdatedByUserNameSnapshot = "Megan Bowel",
                    SizeBytes = 0,
                    StorageKey = string.Empty,
                    Checksum = string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new FilePersistence
                {
                    Id = Guid.NewGuid(),
                    Name = "RevenueByServices2017",
                    Extension = "xlsx",
                    Path = "/",
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    FolderId = rootFolderId,
                    CreatedByUserId = CoreConstants.DEFAULT_USER_ID,
                    CreatedByUserNameSnapshot = "Megan Bowel",
                    UpdatedByUserId = CoreConstants.DEFAULT_USER_ID,
                    UpdatedByUserNameSnapshot = "Megan Bowel",
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
