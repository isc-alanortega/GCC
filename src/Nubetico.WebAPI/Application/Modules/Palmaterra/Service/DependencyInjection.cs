using FluentValidation;
using Nubetico.WebAPI.Application.Modules.Palmaterra.Service;
namespace Nubetico.WebAPI.Application.Modules.Palmaterra
{
    public static class DependencyInjection 
    {
        public static IServiceCollection AddPalmaterraServices(this IServiceCollection services)
        {
            #region SERVICES
            services.AddTransient<PalmaterraPieceworkService>();
			services.AddTransient<PalmaterraService>();
			#endregion

			#region VALIDATORS
			//services.AddScoped<IValidator<ProjectDataDto>, ProjectDtoValidator>();
			#endregion

			#region MAPPERS
			//services.AddAutoMapper(typeof(ProjectSectionProfile));

			#endregion

			return services;
        }
    }
}
