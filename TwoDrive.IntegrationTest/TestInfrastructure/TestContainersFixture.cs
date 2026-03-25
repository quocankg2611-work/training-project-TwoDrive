using Testcontainers.Azurite;
using Testcontainers.MsSql;

namespace TwoDrive.IntegrationTest.TestInfrastructure;

public sealed class TestContainersFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder().Build();
    private readonly AzuriteContainer _azuriteContainer = new AzuriteBuilder().Build();

    public string SqlConnectionString => _sqlContainer.GetConnectionString();
    public string AzuriteConnectionString => _azuriteContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        await _azuriteContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _azuriteContainer.DisposeAsync();
        await _sqlContainer.DisposeAsync();
    }
}
