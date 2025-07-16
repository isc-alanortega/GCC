using Nubetico.WebAPI.Application.Modules.Palmaterra.Service;
using Nubetico.WebAPI.Application.Modules.PortalClientes.Services;

namespace Nubetico.WebAPI.Application.Modules.PortalClientes
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddPortalClientesServices(this IServiceCollection services)
        {
            #region SERVICES
            services.AddTransient<ClientInvoicesService>();
            #endregion

            #region VALIDATORS

            #endregion

            return services;
        }
    }
}

