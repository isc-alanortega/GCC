//using Nubetico.Frontend.Services.ProveedoresFacturas;
//using Nubetico.Frontend.Services.ProveedoresFacturas;
using Nubetico.WebAPI.Application.Modules.Core.Services;
using Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services;
using Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices;
using Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices.Interfaces;
using Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices.SendInvoice;
using Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices.Validator;

namespace Nubetico.WebAPI.Application.Modules.ProveedoresFacturas
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProveedoresFacturasServices(this IServiceCollection services)
        {
            services.AddTransient<FacturasServices>();
            services.AddTransient<ClientesServices>();

            // Registers the InvoiceServiceFactory as a singleton, so the same instance of IInvoiceServiceFactory
            // is used throughout the application's lifetime
            services.AddTransient<UploadInvoiceService>();

            #region MOREGAR
            services.AddTransient<MoregarInvoiceValidator>();
            services.AddTransient<MoregarSendInvoice>();
            #endregion

            services.AddScoped<IInvoiceServiceFactory, InvoiceServiceFactory>();

            return services;
        }
    }
}
