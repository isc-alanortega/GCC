using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.PortalProveedores;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.WebAPI.Application.External;
using Nubetico.WebAPI.Application.Modules.Core;
using Nubetico.WebAPI.Application.Modules.Forwarders;
using Nubetico.WebAPI.Application.Modules.Palmaterra;
using Nubetico.WebAPI.Application.Modules.PortalClientes;
using Nubetico.WebAPI.Application.Modules.PortalProveedores;
using Nubetico.WebAPI.Application.Modules.ProveedoresFacturas;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Application
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
        {
            services.AddExternal(config);
            services.AddScoped<JwtHandlerService>();
            services.AddCoreServices();
            services.AddForwardersServices();
            services.AddPalmaterraServices();
            services.AddPortalClientesServices();
            services.AddPortalProveedoresServices();
            services.AddProveedoresFacturasServices();
            services.AddProyectosConstruccionServices();
            
            return services;
        }

        public static IServiceCollection AddDataAccessLayerTenant(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<TenantConnectionService>();

            // DbContextFactory para CoreDbContext
            services.AddDbContextFactory<CoreDbContext>((serviceProvider, options) =>
            {
                var tenantConnectionService = serviceProvider.GetRequiredService<TenantConnectionService>();
                var connectionString = tenantConnectionService.GetTenant()?.ConnectionString;

                if (!string.IsNullOrEmpty(connectionString))
                {
                    options.UseSqlServer(connectionString);
                }
                else
                {
                    throw new InvalidOperationException("Connection string not found for the tenant.");
                }
            }, ServiceLifetime.Scoped);

            // DbContextFactory para ProyectosConstruccionDbContext
            services.AddDbContextFactory<ProyectosConstruccionDbContext>((serviceProvider, options) =>
            {
                var tenantConnectionService = serviceProvider.GetRequiredService<TenantConnectionService>();
                var connectionString = tenantConnectionService.GetTenant()?.ConnectionString;

                if (!string.IsNullOrEmpty(connectionString))
                {
                    options.UseSqlServer(connectionString);
                }
                else
                {
                    throw new InvalidOperationException("Connection string not found for the tenant.");
                }
            }, ServiceLifetime.Scoped);

            // DbContextFactory para DbContextFactory para PortalProveedoresContext
            //services.AddDbContextFactory<PortalClientesContext>((serviceProvider, options) =>
            //{
            //    var tenantConnectionService = serviceProvider.GetRequiredService<TenantConnectionService>();
            //    var connectionString = tenantConnectionService.GetTenant()?.ConnectionString;

            //    if (!string.IsNullOrEmpty(connectionString))
            //    {
            //        options.UseSqlServer(connectionString);
            //    }
            //    else
            //    {
            //        throw new InvalidOperationException("Connection string not found for the tenant.");
            //    }
            //}, ServiceLifetime.Scoped);

            // DbContextFactory para PortalProveedoresContext
            services.AddDbContextFactory<PortalProveedoresContext>((serviceProvider, options) =>
            {
                var tenantConnectionService = serviceProvider.GetRequiredService<TenantConnectionService>();
                var connectionString = tenantConnectionService.GetTenant()?.ConnectionString;

                if (!string.IsNullOrEmpty(connectionString))
                {
                    options.UseSqlServer(connectionString);
                }
                else
                {
                    throw new InvalidOperationException("Connection string not found for the tenant.");
                }
            }, ServiceLifetime.Scoped);

            return services;
        }
    }
}
