using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TwoDrive.IntegrationTest.TestInfrastructure;

public sealed class TwoDriveApiFactory(TestContainersFixture testContainersFixture) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = testContainersFixture.SqlConnectionString,
                ["StorageConnectionString:blobServiceUri"] = testContainersFixture.AzuriteConnectionString,
                ["StorageConnectionString:queueServiceUri"] = testContainersFixture.AzuriteConnectionString,
                ["StorageConnectionString:tableServiceUri"] = testContainersFixture.AzuriteConnectionString,
                ["AzureAd:Instance"] = "https://login.microsoftonline.com/",
                ["AzureAd:Domain"] = "example.onmicrosoft.com",
                ["AzureAd:TenantId"] = "00000000-0000-0000-0000-000000000000",
                ["AzureAd:ClientId"] = "00000000-0000-0000-0000-000000000001"
            });
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContextType = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(assembly => assembly.GetName().Name == "TwoDrive.Persistence")
            ?.GetType("TwoDrive.Persistence.AppDbContext");

        if (dbContextType is null)
        {
            throw new InvalidOperationException("Cannot resolve AppDbContext type for integration test setup.");
        }

        var dbContext = scope.ServiceProvider.GetRequiredService(dbContextType);
        var databaseFacade = dbContextType.GetProperty("Database")!.GetValue(dbContext)!;
        var ensureCreatedAsync = databaseFacade.GetType().GetMethod("EnsureCreatedAsync", [typeof(CancellationToken)]);

        if (ensureCreatedAsync is null)
        {
            throw new InvalidOperationException("Cannot invoke EnsureCreatedAsync on test database.");
        }

        await (Task)ensureCreatedAsync.Invoke(databaseFacade, [CancellationToken.None])!;
    }
}
