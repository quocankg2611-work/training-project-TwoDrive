using Microsoft.Extensions.DependencyInjection;
using TwoDrive.Services.__Services__;

namespace TwoDrive.Http
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTwoDriveHttp(this IServiceCollection services)
        {
            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            return services;
        }
    }
}
