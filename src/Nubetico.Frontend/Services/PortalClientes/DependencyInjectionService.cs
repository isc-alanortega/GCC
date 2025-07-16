namespace Nubetico.Frontend.Services.PortalClientes
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddPortalClientesServices(this IServiceCollection services)
        {
            services.AddScoped<ClientInvoicesService>();

            return services;
        }
    }
}
