using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwoDrive.Persistence.Common;
using TwoDrive.Persistence.Helpers;
using TwoDrive.Persistence.Repositories;
using TwoDrive.Services.__Persistence__;
using TwoDrive.Services.Common;

namespace TwoDrive.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTwoDrivePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(Constants.CONNECTION_STRING_KEY);

            services.AddSingleton<AuditSaveChangesInterceptor>();
            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                options
                    .UseSqlServer(connectionString)
                    .AddInterceptors(serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>());
            });

            services.AddScoped<PathMaintenanceHelper>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.Scan(scan => scan
                .FromAssemblies(typeof(DependencyInjection).Assembly)
                .AddClasses(classes => classes.AssignableToAny(typeof(IRepository)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.Scan(scan => scan
                .FromAssemblies(typeof(DependencyInjection).Assembly)
                .AddClasses(classes => classes.AssignableToAny(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}
