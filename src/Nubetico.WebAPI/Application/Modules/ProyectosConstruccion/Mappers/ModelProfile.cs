using AutoMapper;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Models;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Mappers
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            #region FROM Modelos
            // map from Modelos to ModelDto
            CreateMap<Modelos, ModelDto>()
                .ForMember(dest => dest.ModelId, opt => opt.MapFrom(src => src.ID_Modelo))
                .ForMember(dest => dest.Folio, opt => opt.MapFrom(src => src.Folio))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.ModelGuid, opt => opt.MapFrom(src => src.UUID));
            //.ForMember(dest => dest.Enabled, opt => opt.MapFrom(src => src.Habilitado));

            // map from Modelos to ModelGridDto
            CreateMap<Modelos, ModelGridDto>()
                .ForMember(dest => dest.ModelId, opt => opt.MapFrom(src => src.ID_Modelo))
                .ForMember(dest => dest.Folio, opt => opt.MapFrom(src => src.Folio))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Nombre));
            #endregion

            #region FROM ModelDto
            // map from ModelDto to Modelos
            CreateMap<ModelDto, Modelos>()
                .ForMember(dest => dest.Folio, opt => opt.MapFrom(src => src.Folio))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Description));
            #endregion

            #region DETAIL MODELS GROUPS
            
            #endregion

            #region MODELOS_GRUPOS
            CreateMap<ModelGroupDto, Modelos_Grupos>()
                .ForMember(dest => dest.ID_Modelo_Grupo, opt => opt.MapFrom(src => src.GroupId))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Habilitado, opt => opt.MapFrom(src => src.Enabled));
            #endregion

            #region MODELOS_PARTIDAS
            CreateMap<ModelBatchDto, Modelos_Partidas>()
                .ForMember(dest => dest.ID_Modelo_Partida, opt => opt.MapFrom(src => src.BatchId))
                .ForMember(dest => dest.Id_Modelo_Grupo, opt => opt.MapFrom(src => src.ModelGroupId))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Habilitado, opt => opt.MapFrom(src => src.Enabled));
            #endregion

            #region MODELOS_PRECIO_UNITARIO
            CreateMap<ModelUnitPriceDto, Modelos_Precios_Unitarios>()
                .ForMember(dest => dest.ID_Modelo_Precio_Unitario, opt => opt.MapFrom(src => src.PriceUnitId))
                .ForMember(dest => dest.Id_Modelo_Partida, opt => opt.MapFrom(src => src.ModelBatchId))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Habilitado, opt => opt.MapFrom(src => src.Enabled));
            #endregion

            #region REL_MODELO_PRECIO_UNITARIO_INSUMO
            CreateMap<ModelUnitPriceSupplyDto, Relacion_Precios_Unitarios_Insumos>()
                .ForMember(dest => dest.Id_Modelo_Precio_Unitario, opt => opt.MapFrom(src => src.UnitPriceModelId))
                .ForMember(dest => dest.Id_Insumo, opt => opt.MapFrom(src => src.SupplyId))
                .ForMember(dest => dest.Volumen, opt => opt.MapFrom(src => src.Volume))
                .ForMember(dest => dest.Precio, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Id_Unidad, opt => opt.MapFrom(src => src.UnitId))
                .ForMember(dest => dest.Habilitado, opt => opt.MapFrom(src => src.Enabled));
            #endregion

            #region INSUMOSMODDELOS 
            CreateMap<vModelosExplosionInsumos, InsumosModelos>()
             .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Grupo))
             .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Partida))
             .ForMember(dest => dest.Unit_Price_Id, opt => opt.MapFrom(src => src.ID_Precio_Unitario))
             .ForMember(dest => dest.Unit_Price, opt => opt.MapFrom(src => src.Precio_Unitario))
             .ForMember(dest => dest.Supply, opt => opt.MapFrom(src => src.Insumo))
             .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Tipo))
             .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unidad))
             .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volumen))
             .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Precio));
            #endregion

            CreateMap<InsumosModelos, ModelUnitPriceSupplyDto>()
               .ForMember(dest => dest.Supply, opt => opt.MapFrom(src => src.Supply))
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
               .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
               .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
               .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
               .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<IGrouping<string, InsumosModelos>, ModelBatchDto>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.Enabled, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.UnitPrices, opt => opt.MapFrom(src => src.GroupBy(item => item.Unit_Price)));

            CreateMap<IGrouping<decimal, InsumosModelos>, ModelUnitPriceDto>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.Enabled, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Supplies, opt => opt.MapFrom(src => src.Select(item => item)));

            CreateMap<IGrouping<string, InsumosModelos>, ModelGroupDto>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.Enabled, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Batches, opt => opt.MapFrom(src => src.GroupBy(item => item.Category)));
        }
    }
}
