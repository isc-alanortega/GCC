namespace Nubetico.Frontend.Services.PortalProveedores
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddPortalProveedoresServices(this IServiceCollection services)
        {
            services.AddScoped<ObraService>();

            return services;
        }
    }
}
