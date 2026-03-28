using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using TwoDrive.Services.__Services__;

namespace TwoDrive.Storage;

/// <summary>
/// Extension methods for registering storage services in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddTwoDriveStorageServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(configuration["StorageConnectionString:blobServiceUri"]!);
            clientBuilder.AddQueueServiceClient(configuration["StorageConnectionString:queueServiceUri"]!);
            clientBuilder.AddTableServiceClient(configuration["StorageConnectionString:tableServiceUri"]!);
        });


        services.AddScoped<IFileStorageService, AzureFileStorageService>();
        return services;
    }
}
