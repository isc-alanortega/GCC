using AutoMapper;
using Azure.Core;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Localization;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.DAL.Providers.Core;
using Nubetico.DAL.Providers.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Supplies;
using Nubetico.Shared.Enums.ProyectosConstruccion;
using System;
using System.ComponentModel;
using System.Data;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services
{
    public class SuppliesService(
		IDbContextFactory<ProyectosConstruccionDbContext> dbContextFactory,
        IDbContextFactory<CoreDbContext> coreDbContextFactory,
        IStringLocalizer<SharedResources> localizer,
        IMapper	mapper
	)
    {
		private readonly IDbContextFactory<ProyectosConstruccionDbContext> _dbContextFactory = dbContextFactory;
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory = coreDbContextFactory;
        private readonly IStringLocalizer<SharedResources> _localizer = localizer;
        private readonly IMapper _mapper = mapper;
		
		private async Task<int> GetIdUserAsync(Guid userGuid)
		{
			using (var db = _coreDbContextFactory.CreateDbContext())
			{
				var user = await db.Usuarios.FirstOrDefaultAsync(item => item.GuidUsuarioDirectory == userGuid);
				return user?.IdPersona ?? 0;
			}
		}

		private async Task MapEditData(Insumos suppliesDb, Guid userGuid)
		{
            suppliesDb.Fecha_Modificacion = DateTime.Now;
            suppliesDb.Id_Usuario_Modificacion = await GetIdUserAsync(userGuid);
        }

		private async Task<int> GetTypeIdByNameAsync(string typeName)
		{
			using (var db = _dbContextFactory.CreateDbContext())
			{
				var type = await db.Catalogos.FirstAsync(item => item.Descripcion.ToLower().Contains(typeName.ToLower()));
				return Convert.ToInt32(type.Valor);
			}
		}


        public async Task<PaginatedListDto<InsumosDto>> GetPaginatedSupplies(int limit, int offset, string? code, string? description, int? typeId)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
                var query = context.vInsumos
                          .Where(item => item.Habilitado) // Solo habilitados
                          .AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrWhiteSpace(code))
                    query = query.Where(item => item.Codigo.ToUpper().Contains(code.ToUpper()));

                if (!string.IsNullOrWhiteSpace(description))
                    query = query.Where(item => item.Descripcion.ToUpper().Contains(description.ToUpper()));

                if (typeId.HasValue)
                    query = query.Where(item => item.Id_Tipo == typeId);

                // Obtener el total sin paginar
                var totals = await query.CountAsync();

                // Aplicar ordenamiento
                query = query.OrderBy(item => item.Codigo); // Orden por defecto

                // Aplicar paginación
                var supplies = await query
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();

                return new PaginatedListDto<InsumosDto>
                {
                    RecordsTotal = totals,
                    Data = _mapper.Map<List<InsumosDto>>(supplies),
                    RecordsFiltered = supplies.Count
                };
            }
		}

		public async Task<SuppliesFetcherDto> GetFetchForm()
		{
			using(var db = _dbContextFactory.CreateDbContext())
			{
				var groupsSupplies = await db.Catalogos.Where(item => item.Id_Grupo == (int)GroupsCatalog.TypesSupplies).ToListAsync();
				var umSupplies = await db.Catalogos.Where(item => item.Id_Grupo == (int)GroupsCatalog.UmSupplies).ToListAsync();

				return new()
				{
					TypesSupplies = _mapper.Map<IEnumerable<GroupsCatalogDto>>(groupsSupplies),
					UmSupplies = _mapper.Map<IEnumerable<GroupsCatalogDto>>(umSupplies)
				};
			}
		}

		public async Task<ResponseDto<SuppliesDto>> GetSuppliesByIdAsync(int suppliesId)
		{
			using(var db = _dbContextFactory.CreateDbContext())
			{
				var suppliesDb = await db.Insumos.FirstOrDefaultAsync(item => item.ID == suppliesId);
				if(suppliesDb == null)
					return new(success:  false, message: _localizer["No fue posible abrir el insumo"]);

				var suppliesDto = _mapper.Map<SuppliesDto>(suppliesDb);
				return new ResponseDto<SuppliesDto>(success: true, data: suppliesDto);
			}
		}

        public async Task<ResponseDto<SuppliesDto>> GetSuppliesByNameAsync(string supplyName)
        {
            using (var db = _dbContextFactory.CreateDbContext())
            {
                var suppliesDb = await db.Insumos.FirstOrDefaultAsync(item => item.Descripcion == supplyName);
                if (suppliesDb == null)
                    return new(success: false, message: _localizer["No fue posible abrir el insumo"]);

                var suppliesDto = _mapper.Map<SuppliesDto>(suppliesDb);
                return new ResponseDto<SuppliesDto>(success: true, data: suppliesDto);
            }
        }

        public async Task<ResponseDto<SuppliesDto>> AddAsync(SuppliesDto request)
		{
			using(var db = _dbContextFactory.CreateDbContext())
			{
				var suppliesDb = _mapper.Map<Insumos>(request);
				suppliesDb.Id_Usuario_Alta = await GetIdUserAsync(request.ActionUserGuid!.Value);
				suppliesDb.Fecha_Alta = DateTime.Now;
				suppliesDb.Habilitado = true;

				await db.AddAsync(suppliesDb);
				await db.SaveChangesAsync();

				return new ResponseDto<SuppliesDto>(success: true, data: request);
            }
		}

        public async Task<(bool succes, int supplyId)> AddAsync(string nameSupply, string typeName, int userId)
        {
			try
			{
                using (var db = _dbContextFactory.CreateDbContext())
                {
                    var typeId = await GetTypeIdByNameAsync(typeName);

                    var supplyDto = new SuppliesDto()
                    {
                        Active = true,
                        Description = nameSupply,
                        Code = "-",
                        TypeId = typeId
                    };

                    var supplyDb = _mapper.Map<Insumos>(supplyDto);
                    supplyDb.Id_Usuario_Alta = userId;
                    supplyDb.Fecha_Alta = DateTime.Now;
                    supplyDb.Habilitado = true;

                    await db.AddAsync(supplyDb);
                    await db.SaveChangesAsync();

                    return (succes: true, supplyId: supplyDb.ID);
                }
            }
			catch(Exception ex)
			{
                return (succes: true, supplyId: 0);
            }

        }

        public async Task<ResponseDto<SuppliesDto>> EditAsync(SuppliesDto request)
        {
            using (var db = _dbContextFactory.CreateDbContext())
            {
				var suppliesDb = await db.Insumos.FirstOrDefaultAsync(item => item.ID == request.SuppliesId);
				if (suppliesDb == null)
					return new ResponseDto<SuppliesDto>(success: false, _localizer["No fue posible acualizar el insumo"], request);

				_mapper.Map(request, suppliesDb);
                await MapEditData(suppliesDb, request.ActionUserGuid!.Value);

                await db.SaveChangesAsync();

                return new ResponseDto<SuppliesDto>(success: true, data: request, message: _localizer["Insumo actualizado correctamente"]);
            }
        }

		public async Task<ResponseDto<object>> DeleteAsync(int suppliesId, Guid userGuid)
		{
			using(var db = _dbContextFactory.CreateDbContext())
			{
				var suppliesDb = await db.Insumos.FirstOrDefaultAsync(item => item.ID == suppliesId);
				if(suppliesDb == null)
					return new ResponseDto<object>(success: false, data: null, message: _localizer["No fue posible eliminar el insumo"]);

				await MapEditData(suppliesDb, userGuid);
				suppliesDb.Habilitado = false;

				await db.SaveChangesAsync();

				return new ResponseDto<object>(success: true, data: null, message: _localizer["Insumo eliminado correctamente"]);
			}
		}
        public async Task<List<TablaRelacionDto>> GetAllTipoInsumo()
        {
            var result = await InsumosProvider.GetAllTipoInsumo(_dbContextFactory);
            return result;
        }
        public async Task<List<TablaRelacionDto>> GetAllInsumos()
        {
            var result = await InsumosProvider.GetAllInsumos(_dbContextFactory);
            return result;
        }
    }
}
