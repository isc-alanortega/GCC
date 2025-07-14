using AutoMapper;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Mappers.Project
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<vProyectos, ProyectsGridDto>()
                .ForMember(dest => dest.ProjectGuid, opt => opt.MapFrom(src => src.ProyectoGuid.HasValue ? src.ProyectoGuid.Value : Guid.Empty))
                .ForMember(dest => dest.BusinessUnit, opt => opt.MapFrom(src => src.Unidad_Negocio))
                .ForMember(dest => dest.Subdivision, opt => opt.MapFrom(src => src.Fraccionamiento))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TipoProyecto))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Proyecto))
                .ForMember(dest => dest.ProjectedStartDate, opt => opt.MapFrom(src => src.Fecha_Inicio_Proyectada != null ? src.Fecha_Inicio_Proyectada.Value.ToString("dd/MM/yyyy") : null))
                .ForMember(dest => dest.ProjectedEndDate, opt => opt.MapFrom(src => src.Fecha_Terminacion_Proyectada != null ? src.Fecha_Terminacion_Proyectada.Value.ToString("dd/MM/yyyy") : null))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.Estatus))
                .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.IdEstatus))
                .ForMember(dest => dest.Folio, opt => opt.MapFrom(src => src.Folio));

            CreateMap<Proyectos, ProjectDataDto>()
                .ForMember(dest => dest.ProjectGuid, opt => opt.MapFrom(src => src.UUID))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Id_Proyecto))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.Id_Sucursal))
                .ForMember(dest => dest.SubdivisionId, opt => opt.MapFrom(src => src.Id_Fraccionamiento))
                .ForMember(dest => dest.TypeId, opt => opt.MapFrom(src => src.Id_Tipo))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.ResponsibleId, opt => opt.MapFrom(src => src.Id_Encargado))
                .ForMember(dest => dest.ProjectedStartDate, opt => opt.MapFrom(src => src.Fecha_Inicio_Proyectada))
                .ForMember(dest => dest.ProjectedEndDate, opt => opt.MapFrom(src => src.Fecha_Terminacion_Proyectada))
                .ForMember(dest => dest.ActualStartDate, opt => opt.MapFrom(src => src.Fecha_Inicio))
                .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(src => src.Fecha_Terminacion))
                .ForMember(dest => dest.Folio, opt => opt.MapFrom(src => src.Folio))
                .ForMember(dest => dest.TotalUnits, opt => opt.MapFrom(src => src.Total_Unidades))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.Id_Estado));

            CreateMap<ProjectDataDto, Proyectos>()
                .ForMember(dest => dest.Id_Sucursal, opt => opt.MapFrom(src => src.BranchId))
                .ForMember(dest => dest.Id_Fraccionamiento, opt => opt.MapFrom(src => src.SubdivisionId))
                .ForMember(dest => dest.Id_Tipo, opt => opt.MapFrom(src => src.TypeId))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Id_Encargado, opt => opt.MapFrom(src => src.ResponsibleId))
                .ForMember(dest => dest.Fecha_Inicio_Proyectada, opt => opt.MapFrom(src => src.ProjectedStartDate!.Value))
                .ForMember(dest => dest.Fecha_Terminacion_Proyectada, opt => opt.MapFrom(src => src.ProjectedEndDate!.Value))
                .ForMember(dest => dest.Fecha_Inicio, opt => opt.MapFrom(src => src.ActualStartDate))
                .ForMember(dest => dest.Fecha_Terminacion, opt => opt.MapFrom(src => src.ActualEndDate))
                .ForMember(dest => dest.Folio, opt => opt.MapFrom(src => src.Folio))
                .ForMember(dest => dest.Total_Unidades, opt => opt.MapFrom(src => src.TotalUnits))
                .ForMember(dest => dest.Id_Estado, opt => opt.MapFrom(src => src.StatusId));
        }
    }
}
