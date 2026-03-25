using Microsoft.Extensions.DependencyInjection;
using TwoDrive.Services.__Services__;

namespace TwoDrive.Storage;

/// <summary>
/// Extension methods for registering storage services in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds storage services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddTwoDriveStorageServices(this IServiceCollection services)
    {
        services.AddScoped<IFileStorageService, AzureFileStorageService>();
        return services;
    }
}
