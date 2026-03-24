using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwoDrive.Persistence.Common;
using TwoDrive.Persistence.Helpers;
using TwoDrive.Services.Common;

namespace TwoDrive.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddSingleton<AuditSaveChangesInterceptor>();

            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
                options
                    .UseSqlServer(connectionString)
                    .AddInterceptors(serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

            services.AddScoped<PathMaintenanceHelper>();

                services.Scan(scan => scan
                    .FromAssemblies(typeof(DependencyInjection).Assembly)
                    .AddClasses(classes => classes.AssignableToAny(
                        typeof(ICommandHandler<>),
                        typeof(ICommandHandler<,>),
                        typeof(IQueryHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());

            return services;
        }
    }
}
