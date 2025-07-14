using Nubetico.Frontend.Services.ProveedoresFacturas;

namespace Nubetico.Frontend.Services.ProveedoresFacturas
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddProveedoresFacturasServices(this IServiceCollection services)
        {
            services.AddScoped<FacturasService>();

            return services;
        }
    }
}
