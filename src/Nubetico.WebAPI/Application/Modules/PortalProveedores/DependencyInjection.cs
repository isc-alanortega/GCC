using Nubetico.WebAPI.Application.Modules.PortalProveedores.Services;

namespace Nubetico.WebAPI.Application.Modules.PortalProveedores
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPortalProveedoresServices(this IServiceCollection services)
        {
            services.AddTransient<ObrasService>();
            return services;
        }
    }
}
