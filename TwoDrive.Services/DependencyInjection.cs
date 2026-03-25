using Microsoft.Extensions.DependencyInjection;
using TwoDrive.Services.Common;

namespace TwoDrive.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddTwoDriveServices(this IServiceCollection services)
    {
        services
            .Scan(scan => scan
            .FromAssemblies(typeof(DependencyInjection).Assembly)
            .AddClasses(classes => classes.AssignableToAny(
                typeof(ICommandHandler<>),
                typeof(ICommandHandler<,>)),
                publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}
