//using Nubetico.Frontend.Services.ProyectosConstruccion;

namespace Nubetico.Frontend.Services.Palmaterra
{
	public static class DependencyInjectionService
	{
		public static IServiceCollection AddPalmaterraServices(this IServiceCollection services)
		{
			services.AddScoped<ObrasServices>();

			return services;
		}
	}
}
