using FluentValidation;
using Nubetico.Shared.Dto.Core;
using Nubetico.WebAPI.Application.Modules.Core.Services;
using Nubetico.WebAPI.Application.Modules.Core.Validators;

namespace Nubetico.WebAPI.Application.Modules.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            // Servicios
            services.AddTransient<DocumentosService>();
            services.AddTransient<DomiciliosService>();
            services.AddTransient<FormsService>();
            services.AddTransient<MenusService>();
            services.AddTransient<ParametrosService>();
            services.AddTransient<PermisosService>();
            services.AddTransient<SucursalesService>();
            services.AddTransient<TenantsService>();
            services.AddTransient<UsuariosService>();
            services.AddTransient<EntidadesService>();

            // Validators
            services.AddScoped<IValidator<AuthRequestDto>, AuthDtoValidator>();
            services.AddScoped<IValidator<UpdatePswdByTokenDto>, UpdatePswdByTokenDtoValidator>();
            services.AddScoped<IValidator<UserTwoFactorCodeDto>, UserTwoFactorCodeDtoValidator>();
            services.AddScoped<IValidator<UsuarioNubeticoDto>, UsuarioNubeticoDtoValidator>();

            return services;
        }
    }
}
