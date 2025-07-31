using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.DAL.Providers.Core;
using Nubetico.DAL.ResultSets.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Contratistas;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services
{
    public class ContratistasService
    {

        private readonly IDbContextFactory<ProyectosConstruccionDbContext> _dbContextFactory;
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;

        public ContratistasService(IDbContextFactory<ProyectosConstruccionDbContext> dbContextFactory, IDbContextFactory<CoreDbContext> coreDbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _coreDbContextFactory = coreDbContextFactory;

        }
        #region POST
        public async Task<BaseResponseDto<ContratistaResult>> CreateContratistaAsync(ContratistasDto contratistaRequest)
        {
            try
            {
                using var coreDbContext = await _coreDbContextFactory.CreateDbContextAsync();
                var result = await ContratistasProvider.CreateContratistaAsync(_coreDbContextFactory, contratistaRequest);
                if (result == null)
                {
                    return new BaseResponseDto<ContratistaResult>
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "No se obtuvo una respuesta del procedimiento almacenado.",
                        ResponseKey = Guid.NewGuid(),
                        Data = null
                    };
                }

                if (!result.bResult)
                {
                    return new BaseResponseDto<ContratistaResult>
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result.vchMessage,
                        ResponseKey = Guid.NewGuid(),
                        Data = null
                    };
                }
                

                return new BaseResponseDto<ContratistaResult>
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
                return new BaseResponseDto<ContratistaResult>
                {
                    StatusCode = 500,
                    Success = false,
                    Message = $"Error al crear el contratista: {ex.Message}",
                    ResponseKey = Guid.NewGuid(),
                    Data = null
                };
            }
        }
        #endregion
        #region GET
        public async Task<BaseResponseDto<PaginatedListDto<ContratistaGridResultSet>>> GetContratistasPaginadoAsync(int limit, int offset, string? orderBy, string? nombre, string? rfc)
        {
            try
            {
                // Llamada directa al contratista en lugar de una solicitud HTTP
                using var coreDbContext = await _coreDbContextFactory.CreateDbContextAsync();
                var result = await ContratistasProvider.GetContratistasPaginadoAsync(_coreDbContextFactory, limit, offset, orderBy, nombre, rfc);
                if (result.Data == null || !result.Data.Any())
                {
                    return new BaseResponseDto<PaginatedListDto<ContratistaGridResultSet>>
                    {
                        Success = false,
                        Message = "No se encontraron contratistas.",
                        Data = new PaginatedListDto<ContratistaGridResultSet>
                        {
                            Data = new List<ContratistaGridResultSet>()
                        }
                    };
                }
                return new BaseResponseDto<PaginatedListDto<ContratistaGridResultSet>>
                {
                    Success = true,
                    Message = "Contratistas encontrados.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<PaginatedListDto<ContratistaGridResultSet>>
                {
                    Success = false,
                    Message = $"Error al obtener los contratistas: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ContratistasDto?> GetContratistaByIdAsync(int idContratista)
        {
            using var coreDbContext = await _coreDbContextFactory.CreateDbContextAsync();
            var result = await ContratistasProvider.GetContratistaByIdAsync(_coreDbContextFactory, idContratista);
            return result?.FirstOrDefault(); // Devuelve un solo registro o null si no hay resultados
        }
        #endregion
        #region PUT
        public async Task<BaseResponseDto<ContratistaResult>> PutSaveContratista(ContratistasDto request)
        {
            try
            {
                using var coreDbContext = await _coreDbContextFactory.CreateDbContextAsync();
                var result = await ContratistasProvider.PutSaveContratista(_coreDbContextFactory, request);

                if (result == null || !result.bResult)
                {
                    return new BaseResponseDto<ContratistaResult>
                    {
                        StatusCode = 400,
                        Success = false,
                        Message = result?.vchMessage ?? "Error al actualizar el contratista.",
                        ResponseKey = Guid.NewGuid()
                    };
                }

                return new BaseResponseDto<ContratistaResult>
                {
                    StatusCode = 200,
                    Success = true,
                    Message = result.vchMessage,
                    Data = result,
                    ResponseKey = Guid.NewGuid()
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<ContratistaResult>
                {
                    StatusCode = 500,
                    Success = false,
                    Message = $"Error al actualizar el contratista: {ex.Message}, Inner: {ex.InnerException?.Message}",
                    ResponseKey = Guid.NewGuid()
                };
            }
        }

        #endregion
    }
}
