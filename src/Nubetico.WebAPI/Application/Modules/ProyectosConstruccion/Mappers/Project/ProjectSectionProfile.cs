using AutoMapper;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Mappers.Project
{
    public class ProjectSectionProfile : Profile
    {
        public ProjectSectionProfile()
        {
            // Configurar el mapeo de ProjectSectionDataDto a Secciones
            CreateMap<ProjectSectionDataDto, Secciones>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Id_Contratista, opt => opt.MapFrom(src => src.GeneralContractor))
                .ForMember(dest => dest.Fecha_Inicio_Proyectada, opt => opt.MapFrom(src => src.ProjectedStartDate!.Value))
                .ForMember(dest => dest.Fecha_Terminacion_Proyectada, opt => opt.MapFrom(src => src.ProjectedEndDate!.Value))
                .ForMember(dest => dest.Id_Estado, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Fecha_Inicio, opt => opt.MapFrom(src => src.RealStartDate))
                .ForMember(dest => dest.Fecha_Terminacion, opt => opt.MapFrom(src => src.RealEndDate))
                .ForMember(dest => dest.Secuencia, opt => opt.MapFrom(src => src.Sequence))
                .ForMember(dest => dest.Id_Modelo, opt => opt.MapFrom(src => src.ModelId));

            // Configurar el mapeo de Secciones a ProjectSectionDataDto
            CreateMap<Secciones, ProjectSectionDataDto>()
                .ForMember(dest => dest.SectionId, opt => opt.MapFrom(src => src.Id_Seccion))
                .ForMember(dest => dest.SectionGuid, opt => opt.MapFrom(src => src.UUID))
                .ForMember(dest => dest.Folio, opt => opt.MapFrom(src => src.Folio))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.GeneralContractor, opt => opt.MapFrom(src => src.Id_Contratista))
                .ForMember(dest => dest.ProjectedStartDate, opt => opt.MapFrom(src => src.Fecha_Inicio_Proyectada))
                .ForMember(dest => dest.ProjectedEndDate, opt => opt.MapFrom(src => src.Fecha_Terminacion_Proyectada))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Id_Estado))
                .ForMember(dest => dest.Sequence, opt => opt.MapFrom(src => src.Secuencia))
                .ForMember(dest => dest.RealStartDate, opt => opt.MapFrom(src => src.Fecha_Inicio))
                .ForMember(dest => dest.RealEndDate, opt => opt.MapFrom(src => src.Fecha_Terminacion))
                .ForMember(dest => dest.ModelId, opt => opt.MapFrom(src => src.Id_Modelo))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Habilitado));

            CreateMap<Lotes, SectionLotsGridDto>()
                .ForMember(dest => dest.LotId, opt => opt.MapFrom(src => src.Id_Lote))
                .ForMember(dest => dest.LotNumber, opt => opt.MapFrom(src => src.Numero_Lote))
                .ForMember(dest => dest.IsLotSelected, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.ModelId, opt => opt.MapFrom(src => src.Id_Modelo));

            CreateMap<SectionLotsGridDto, Secciones_Lotes>()
                .ForMember(dest => dest.Id_Lote, opt => opt.MapFrom(src => src.LotId))
                .ForMember(dest => dest.Id_Seccion, opt => opt.MapFrom(src => src.SectionId!.Value))
                .ForMember(dest => dest.Id_Modelo, opt => opt.MapFrom(src => src.ModelId!.Value));
        }
    }
}
