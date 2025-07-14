using AutoMapper;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Mappers.Project
{
    public class ProjectSectionPhaseProfile : Profile
    {
        public ProjectSectionPhaseProfile()
        {
            CreateMap<ProjectSectionPhaseDto, Secciones_Fases>()
                .ForMember(dest => dest.Id_Seccion, opt => opt.MapFrom(src => src.SectionId))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Terminada, opt => opt.MapFrom(src => src.Complete))
                .ForMember(dest => dest.Secuencia, opt => opt.MapFrom(src => src.Sequence));

            CreateMap<Secciones_Fases, ProjectSectionPhaseDto>()
                .ForMember(dest => dest.PhaseId, opt => opt.MapFrom(src => src.Id_Seccion_Fase))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.Complete, opt => opt.MapFrom(src => src.Terminada))
                .ForMember(dest => dest.Sequence, opt => opt.MapFrom(src => src.Secuencia))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Habilitado));
        }
    }
}
