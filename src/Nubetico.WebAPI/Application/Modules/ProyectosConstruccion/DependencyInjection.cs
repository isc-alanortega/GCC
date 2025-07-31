using FluentValidation;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Nubetico.Shared.Dto.ProyectosConstruccion.ReportesDestajos;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Mappers;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Mappers.Project;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services.Projects;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services.ProjectSectionDetails;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Validators;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Validators.Project;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion
{
    public static class DependencyInjection 
    {
        public static IServiceCollection AddProyectosConstruccionServices(this IServiceCollection services)
        {
            #region SERVICES
            services.AddTransient<LotsService>();
            services.AddTransient<ProjectSectionDetailsService>();
            services.AddTransient<ProjectsService>();
            services.AddTransient<ProjectSectionService>();
            services.AddTransient<ProjectSectionPhaseService>();
            services.AddTransient<SubdivisionsService>();
            services.AddTransient<SuppliesService>();
            services.AddTransient<ModelsService>();
            services.AddTransient<CatalogsSupplysService>();
            services.AddTransient<ReportesDestajosService>();
            services.AddTransient<ProveedoresService>();
            services.AddTransient<EgresosService>();
            services.AddTransient<ContratistasService>();
            #endregion

            #region VALIDATORS
            services.AddScoped<IValidator<ProjectDataDto>, ProjectDtoValidator>();
            services.AddScoped<IValidator<ReporteDestajoDto>, ReporteDestajoDtoValidator>();
            #endregion

            #region MAPPERS
            services.AddAutoMapper(typeof(ProjectSectionProfile));
            services.AddAutoMapper(typeof(ProjectSectionPhaseProfile));
            services.AddAutoMapper(typeof(ProjectProfile));
            services.AddAutoMapper(typeof(SuppliesProfile));
            services.AddAutoMapper(typeof(PcGroupsCatalogProfile));
            services.AddAutoMapper(typeof(ModelProfile));
            #endregion

            return services;
        }
    }
}
