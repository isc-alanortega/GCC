using AutoMapper;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Supplies;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Mappers
{
    public class SuppliesProfile : Profile
    {
        public SuppliesProfile()
        {
            CreateMap<SuppliesDto, Insumos>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.SuppliesId))
                .ForMember(dest => dest.Id_Tipo, opt => opt.MapFrom(src => src.TypeId))
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Habilitado, opt => opt.MapFrom(src => src.Active));

            CreateMap<Insumos, SuppliesDto>()
               .ForMember(dest => dest.SuppliesId, opt => opt.MapFrom(src => src.ID))
               .ForMember(dest => dest.TypeId, opt => opt.MapFrom(src => src.Id_Tipo))
               .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Codigo))
               .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Descripcion))
               .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Habilitado));


            CreateMap<vInsumos, InsumosDto>()
               .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
               .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Codigo))
               .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Descripcion))
               .ForMember(dest => dest.Id_Type, opt => opt.MapFrom(src => src.Id_Tipo))
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Tipo_Insumo))
               .ForMember(dest => dest.Enabled, opt => opt.MapFrom(src => src.Habilitado));
        }
    }
}
