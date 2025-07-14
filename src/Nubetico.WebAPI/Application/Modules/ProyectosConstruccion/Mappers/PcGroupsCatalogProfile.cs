using AutoMapper;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Mappers
{
    public class PcGroupsCatalogProfile : Profile
    {
        public PcGroupsCatalogProfile()
        {
            CreateMap<Catalogos, GroupsCatalogDto>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Valor))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.Id_Grupo))
                .ForMember(dest => dest.SubgroupId, opt => opt.MapFrom(src => src.Id_SubGrupo));
        }
    }
}
