using Microsoft.AspNetCore.Components.Authorization;

namespace Nubetico.Frontend.Services.Core
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<AddressesService>();
            services.AddScoped<AuthService>();
            services.AddScoped<AuthStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<AuthStateProvider>());
            services.AddScoped<CustomizationService>();
            services.AddScoped<DocumentsService>();
            services.AddScoped<FormsService>();
            services.AddScoped<GlobalBreakpointService>();
            services.AddScoped<MenuService>();
            services.AddScoped<SucursalesService>();
            services.AddScoped<UsuariosService>();
            services.AddScoped<EntidadesService>();
            services.AddScoped<FoliadorService>();

            return services;
        }
    }
}
