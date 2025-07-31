using System.Net.Http;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Math;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.DAL.Providers.Core;
using Nubetico.DAL.ResultSets.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;
using Nubetico.Shared.Dto.Common;

//using Nubetico.DAL.Models.Core;
//using Nubetico.DAL.Models.ProyectosConstruccion;

//using Nubetico.Shared.Dto.Common;
//using Nubetico.Shared.Dto.ProyectosConstruccion;
//using Nubetico.Shared.Dto.ProyectosConstruccion.Supplies;
//using Nubetico.Shared.Enums.ProyectosConstruccion;


namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services
{
	public class ProveedoresService	{

        private readonly IDbContextFactory<ProyectosConstruccionDbContext> _dbContextFactory;
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;

        public ProveedoresService(IDbContextFactory<ProyectosConstruccionDbContext> dbContextFactory, IDbContextFactory<CoreDbContext> coreDbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _coreDbContextFactory = coreDbContextFactory;

        }
        #region POST
        public async Task<BaseResponseDto<ProveedorResultSet>> CreateProveedorAsync(ProveedorRequestDto proveedorRequest)
        {
            try
            {
                using var coreDbContext = await _coreDbContextFactory.CreateDbContextAsync();
                var result = await ProveedoresProvider.CreateProveedorAsync(_coreDbContextFactory, proveedorRequest);

                if (result == null || !result.bResult)
                {
                    return new BaseResponseDto<ProveedorResultSet>
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result?.vchMessage ?? "Error al crear el proveedor.",
                        ResponseKey = Guid.NewGuid(),
                        Data = null
                    };
                }

                return new BaseResponseDto<ProveedorResultSet>
                {
                    StatusCode = 200,
                    Success = true,
                    Message = result.vchMessage,
                    ResponseKey = Guid.NewGuid(),
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<ProveedorResultSet>
                {
                    StatusCode = 500,
                    Success = false,
                    Message = $"Error al crear el proveedor: {ex.Message}",
                    ResponseKey = Guid.NewGuid(),
                    Data = null
                };
            }
        }
        #endregion


        #region GET
        public async Task<List<ProveedorGridResultSet>> GetAllProveedoresAsync()
        {
            var result = await ProveedoresProvider.GetAllProveedoresAsync(_coreDbContextFactory);
            return result;
        }

        public async Task<ProveedorGetDto?> GetProveedorByIdAsync(int idProveedor)
        {
            using var coreDbContext = await _coreDbContextFactory.CreateDbContextAsync();
            var result = await ProveedoresProvider.GetProveedorByIdAsync(_coreDbContextFactory, idProveedor);
            return result?.FirstOrDefault(); // Devuelve un solo registro o null si no hay resultados
        }

        public async Task<BaseResponseDto<PaginatedListDto<ProveedorGridResultSet>>> GetProveedoresPaginadoAsync(int limit, int offset, string? orderBy, string? nombre, string? rfc)
        {
            try
            {
                // Llamada directa al proveedor en lugar de una solicitud HTTP
                using var coreDbContext = await _coreDbContextFactory.CreateDbContextAsync();
                var result = await ProveedoresProvider.GetProveedoresPaginadoAsync(_coreDbContextFactory, limit, offset, orderBy, nombre, rfc);
                if (result.Data == null || !result.Data.Any())
                {
                    return new BaseResponseDto<PaginatedListDto<ProveedorGridResultSet>>
                    {
                        Success = false,
                        Message = "No se encontraron proveedores.",
                        Data = new PaginatedListDto<ProveedorGridResultSet>
                        {
                            RecordsTotal = 0,
                            Data = new List<ProveedorGridResultSet>()
                        }
                    };
                }
                return new BaseResponseDto<PaginatedListDto<ProveedorGridResultSet>>
                {
                    Success = true,
                    Message = "Proveedores encontrados.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<PaginatedListDto<ProveedorGridResultSet>>
                {
                    Success = false,
                    Message = $"Error al obtener los proveedores: {ex.Message}",
                    Data = null
                };
            }
        }

        #endregion


        #region PUT
        public async Task<BaseResponseDto<ProveedorResultSet>> PutSaveProveedor(ProveedorRequestDto proveedorRequest)
        {
            try
            {
                using var coreDbContext = await _coreDbContextFactory.CreateDbContextAsync();
                var result = await ProveedoresProvider.PutSaveProveedor(_coreDbContextFactory, proveedorRequest);

                if (result == null || !result.bResult)
                {
                    return new BaseResponseDto<ProveedorResultSet>
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result?.vchMessage ?? "Error al actualizar el proveedor.",
                        ResponseKey = Guid.NewGuid(),
                        Data = null
                    };
                }

                return new BaseResponseDto<ProveedorResultSet>
                {
                    StatusCode = 200,
                    Success = true,
                    Message = result.vchMessage,
                    ResponseKey = Guid.NewGuid(),
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<ProveedorResultSet>
                {
                    StatusCode = 500,
                    Success = false,
                    Message = $"Error al actualizar el proveedor: {ex.Message}, InnerException: {ex.InnerException?.Message}",
                    ResponseKey = Guid.NewGuid(),
                    Data = null
                };
            }
        }
        #endregion



    }

}
