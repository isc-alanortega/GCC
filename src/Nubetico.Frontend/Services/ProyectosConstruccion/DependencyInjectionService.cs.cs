namespace Nubetico.Frontend.Services.ProyectosConstruccion
{
	public static class DependencyInjectionService
	{
		public static IServiceCollection AddProyectosConstruccionServices(this IServiceCollection services)
		{
			services.AddScoped<SubdivisionsService>();
			services.AddScoped<ProjectServices>();
			services.AddScoped<ProjectSectionService>();
			services.AddScoped<LotsService>();
			services.AddScoped<SectionDetailsServices>();
			services.AddScoped<SuppliesService>();
			services.AddScoped<ModelService>();

            return services;
		}
	}
}
