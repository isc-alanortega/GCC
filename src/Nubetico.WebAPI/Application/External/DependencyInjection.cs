using Nubetico.WebAPI.Application.External.CIEmail;
using Nubetico.WebAPI.Application.External.Directory;

namespace Nubetico.WebAPI.Application.External
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddExternal(this IServiceCollection services, IConfiguration config)
        {
            services.AddDirectoryServices(config);
            services.AddCIEmailServices(config);

            return services;
        }
    }
}
