using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Models;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services
{
    public class ModelsService(
        IDbContextFactory<ProyectosConstruccionDbContext> dbContextFactory,
        IDbContextFactory<CoreDbContext> coreDbContextFactory,
        IStringLocalizer<SharedResources> localizer,
        CatalogsSupplysService catalogsSupplysService,
        SuppliesService suppliesService,
        IMapper mapper
    )
    {
        private readonly IDbContextFactory<ProyectosConstruccionDbContext> _dbContextFactory = dbContextFactory;
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory = coreDbContextFactory;
        private readonly IStringLocalizer<SharedResources> _localizer = localizer;
        private readonly CatalogsSupplysService _catalogsSupplysService = catalogsSupplysService;
        private readonly SuppliesService _suppliesService = suppliesService;
        private readonly IMapper _mapper = mapper;

        public async Task<PaginatedListDto<ModelGridDto>> GetPaginatedModel(int limit, int offset, string? name)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var query = context.Modelos.AsQueryable();
                var totals = await query.CountAsync();

                if (name != null)
                    query = query.Where(item => item.Nombre.ToUpper().Contains(name.ToUpper()));

                var modelsDb = await query
                            .OrderBy(item => item.Nombre)
                            .Skip(offset)
                            .Take(limit)
                            //.Where(item => item.Habilitado)
                            .ToListAsync();

                return new PaginatedListDto<ModelGridDto>
                {
                    RecordsTotal = totals,
                    Data = _mapper.Map<List<ModelGridDto>>(modelsDb),
                    RecordsFiltered = modelsDb!.Count
                };
            }
        }

        public async Task<ResponseDto<ModelDto>> GetModelByIdAsync(int modelId)
        {
            using (var db = _dbContextFactory.CreateDbContext())
            {
                var modelDb = await db.Modelos.FirstOrDefaultAsync(item => item.ID_Modelo == modelId);
                if (modelDb == null)
                    return new(success: false, message: string.Format(_localizer[$"ProyectosConstruccion.Models.Error.NotFound"], modelId));

                var modelDto = _mapper.Map<ModelDto>(modelDb);

                var modelSupplies = await db.vModelosExplosionInsumos.Where(item => item.ID_Modelo == modelDto.ModelId).ToListAsync();
                modelDto.ModelSupplies = _mapper.Map<List<InsumosModelos>>(modelSupplies);

                return new ResponseDto<ModelDto>(success: true, data: modelDto);
            }
        }

        public async Task<ResponseDto<ModelDto>> AddAsync(ModelDto request)
        {
            using (var db = _dbContextFactory.CreateDbContext())
            {
                var modelDb = _mapper.Map<Modelos>(request);
                modelDb.Id_Usuario_Alta = 1;
                modelDb.Fecha_Alta = DateTime.Now;
                //modelDb.Habilitado = true;

                await db.AddAsync(modelDb);
                await db.SaveChangesAsync();

                _mapper.Map<ModelDto>(request);

                if (request.ModelSupplies.Any())
                {
                    var groupedData = request.ModelSupplies
                    .GroupBy(item => item.Group)
                    .ToDictionary(
                        group => group.Key,
                        group => new ModelGroupDto()
                        {
                            ModelId = modelDb.ID_Modelo,
                            Description = group.Key,
                            Enabled = true,
                            Batches = group.OrderBy(item => item.Category).GroupBy(item => item.Category).ToDictionary(
                                batch => batch.Key,
                                batch => new ModelBatchDto()
                                {
                                    Description = batch.Key,
                                    Enabled = true,
                                    UnitPrices = batch.OrderBy(item => item.Unit_Price).GroupBy(item => item.Unit_Price).ToDictionary(
                                        unitPrice => unitPrice.Key,
                                        unitPrice => new ModelUnitPriceDto()
                                        {
                                            Description = unitPrice.Key,
                                            Enabled = true,
                                            Supplies = unitPrice.Select(item => new ModelUnitPriceSupplyDto()
                                            {
                                                Supply = item.Supply,
                                                Type = item.Type,
                                                Volume = item.Volume,
                                                Unit = item.Unit,
                                                Price = item.Price,
                                                Amount = item.Amount                                                
                                            }).ToList()
                                        }
                                    )
                                }                                    
                            )
                        }
                    );

                    await ModelGroupAddAsync(groupedData);
                }

                return new ResponseDto<ModelDto>(success: true, data: _mapper.Map<ModelDto>(modelDb));
            }
        }

        #region MODEL 
        public async Task<ResponseDto<ModelDto>> EditAsync(ModelDto request)
        {
            using (var db = _dbContextFactory.CreateDbContext())
            {
                var modelDb = await db.Modelos.FirstOrDefaultAsync(item => item.ID_Modelo == request.ModelId);
                if (modelDb == null)
                    return new ResponseDto<ModelDto>(success: false, string.Format(_localizer["ProyectosConstruccion.Models.Error.CantUpdate"], request.ModelGuid), request);

                _mapper.Map(request, modelDb);
                await MapEditData(modelDb, request.UserGuid!.Value);

                await db.SaveChangesAsync();

                return new ResponseDto<ModelDto>(success: true, data: _mapper.Map<ModelDto>(modelDb), message: _localizer["Modelo actualizado correctamente"]);
            }
        }

        public async Task<ResponseDto<object>> DeleteAsync(int modelId, Guid userGuid)
        {
            using (var db = _dbContextFactory.CreateDbContext())
            {
                var modelDb = await db.Modelos.FirstOrDefaultAsync(item => item.ID_Modelo == modelId);
                if (modelDb == null)
                    return new ResponseDto<object>(success: false, data: null, message: _localizer["No fue posible eliminar el insumo"]);

                await MapEditData(modelDb, userGuid);
                //suppliesDb.Habilitado = false;

                await db.SaveChangesAsync();

                return new ResponseDto<object>(success: true, data: null, message: _localizer["Insumo eliminado correctamente"]);
            }
        }

        private async Task MapEditData(Modelos modelDb, Guid userGuid)
        {
            modelDb.Fecha_Modificacion = DateTime.Now;
            modelDb.Id_Usuario_Modificacion = 1;
        }
        #endregion

        #region MODEL GROUPS
        private async Task ModelGroupAddAsync(IDictionary<string, ModelGroupDto> model)
        {
            using (var db = _dbContextFactory.CreateDbContext())
            {
                foreach (var item in model)
                {
                   var modelGroupDb = _mapper.Map<Modelos_Grupos>(item.Value);

                    await db.AddAsync(modelGroupDb);
                    await db.SaveChangesAsync();

                    var detailModelGroupDb = new Relacion_Modelos_Grupos()
                    {
                        Id_Modelo = item.Value.ModelId!.Value,
                        Id_Modelo_Grupo = modelGroupDb.ID_Modelo_Grupo,
                        Fecha_Alta = DateTime.Now,
                        Id_Usuario_Alta = 1,
                        Habilitado = true
                    };

                    await db.AddAsync(detailModelGroupDb);
                    await db.SaveChangesAsync();

                    await ModelBatchAddAsync(item.Value.Batches, modelGroupDb.ID_Modelo_Grupo);
                }
            }
        }
        #endregion

        #region MODEL BATCHES
        private async Task ModelBatchAddAsync(IDictionary<string, ModelBatchDto> batches, int groupId)
        {
            using (var db = _dbContextFactory.CreateDbContext())
            {
                foreach (var item in batches)
                {
                    item.Value.ModelGroupId = groupId;
                    var modelBatchDb = _mapper.Map<Modelos_Partidas>(item.Value);

                    await db.AddAsync(modelBatchDb);
                    await db.SaveChangesAsync();
                    await ModelUnitPriceAddAsync(item.Value.UnitPrices, modelBatchDb.ID_Modelo_Partida);
                }
            }
        }
        #endregion

        #region MODEL UNIT PRICE
        private async Task ModelUnitPriceAddAsync(IDictionary<string, ModelUnitPriceDto> unitPrice, int batchId)
        {
            using (var db = _dbContextFactory.CreateDbContext())
            {
                foreach (var item in unitPrice)
                {
                    item.Value.ModelBatchId = batchId;
                    var unitPriceModelDb = _mapper.Map<Modelos_Precios_Unitarios>(item.Value);
                    

                    try
                    {
                        await db.AddAsync(unitPriceModelDb);
                        await db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine(ex.InnerException.Message);  // Check the inner exception for details
                        }
                    }

                    await ModelUnitPriceSupplyAddAsync(item.Value.Supplies, unitPriceModelDb.ID_Modelo_Precio_Unitario);
                }
            }
        }
        #endregion

        #region UNIT PRICE INSUMOS
        private async Task ModelUnitPriceSupplyAddAsync(List<ModelUnitPriceSupplyDto> supplies, int unitPriceId)
        {
            using (var db = _dbContextFactory.CreateDbContext())
            {
                foreach (var item in supplies)
                {
                    item.UnitPriceModelId = unitPriceId;
                    var unitId = await _catalogsSupplysService.GetTypeSupplyIdByNameAsync(item.Unit!);
                    item.UnitId = (unitId is null || unitId == 0)
                        ? await _catalogsSupplysService.AddUnitSupplyAsync(item.Unit!)
                        : unitId;

                    var supply = await _suppliesService.GetSuppliesByNameAsync(item.Supply!);
                    if (supply.Success)
                    {
                        item.SupplyId = supply!.Result!.SuppliesId!.Value;
                    }
                    else
                    {
                        var (succes, supplyId) = await _suppliesService.AddAsync(item.Supply!, item.Type!, 1);
                        item.SupplyId = supplyId;
                    }

                    var unitPriceModelDb = _mapper.Map<Relacion_Precios_Unitarios_Insumos>(item);
                    unitPriceModelDb.Habilitado = true;

                    await db.AddAsync(unitPriceModelDb);
                    await db.SaveChangesAsync();
                }
            }
        }
        #endregion
    }
}
