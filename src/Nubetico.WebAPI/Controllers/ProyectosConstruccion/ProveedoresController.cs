using Microsoft.AspNetCore.Mvc;
using Nubetico.DAL.Models.Core;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services;
using Nubetico.Shared.Dto.Common;
using Nubetico.WebAPI.Application.Utils;
using System.Threading.Tasks;
using Nubetico.DAL.ResultSets.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;

namespace Nubetico.WebAPI.Controllers.ProyectosConstruccion
{
    [Route("api/v1/proyectosconstruccion/proveedores")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        #region POST
        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ProveedorResultSet>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> CreateProveedor(
    [FromServices] ProveedoresService proveedoresService,
    [FromBody] ProveedorRequestDto proveedorRequest)
        {
            // Validar que el cuerpo de la solicitud no sea nulo
            if (proveedorRequest == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, "El cuerpo de la solicitud no puede ser nulo."));
            }

            try
            {
                var result = await proveedoresService.CreateProveedorAsync(proveedorRequest);

                if (result == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, "Error al ejecutar el procedimiento almacenado."));
                }

                if (!result.Data.bResult)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, result.Data.vchMessage));
                }

                // Devolver todos los datos del proveedor creado
                return Ok(new BaseResponseDto<ProveedorResultSet>
                {
                    StatusCode = 200,
                    Success = true,
                    Message = result.Data.vchMessage,
                    ResponseKey = Guid.NewGuid(),
                    Data = result.Data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, ex.Message));
            }
        }
        #endregion

        #region GET
        [HttpGet("GetAllProveedores")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<List<ProveedorGridResultSet>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetAllProveedores(
    [FromServices] ProveedoresService proveedoresService)
        {
            try
            {
                var result = await proveedoresService.GetAllProveedoresAsync();

                if (result == null || !result.Any())
                {
                    return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, new List<ProveedorGridResultSet>(), "No hay proveedores registrados."));
                }

                return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result, "Proveedores encontrados."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, ex.Message));
            }
        }
        [HttpGet("GetProveedorById{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ProveedorGetDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetProveedorById(
            [FromServices] ProveedoresService proveedoresService,
            [FromRoute] int id)
        {
            try
            {
                var result = await proveedoresService.GetProveedorByIdAsync(id);

                if (result == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound, null, "Proveedor no encontrado."));
                }

                return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result, "Proveedor encontrado."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, ex.Message));
            }
        }
        [HttpGet("GetProveedoresPaginado")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<PaginatedListDto<ProveedorGridResultSet>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetProveedoresPaginado(
                [FromQuery] int? limit,
                [FromQuery] int? offset,
                [FromQuery] string? orderBy,
                [FromQuery] string? nombre,
                [FromQuery] string? rfc,
                [FromServices] ProveedoresService proveedoresService)
        {
            try
            {
                if (!limit.HasValue || !offset.HasValue) return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, "Limit y offset son requeridos."));
                var listaPaginada = await proveedoresService.GetProveedoresPaginadoAsync(limit.Value, offset.Value, orderBy, nombre, rfc);
                if (listaPaginada.Data == null || !listaPaginada.Data.Data.Any()) return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound, null, "No se encontraron proveedores."));
                // Devolver directamente el BaseResponseDto del servicio
                return Ok(listaPaginada);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, ex.Message));
            }
        }
        #endregion

        #region PUT
        [HttpPut("PutSaveProveedor")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ProveedorResultSet>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> PutSaveProveedor(
                [FromServices] ProveedoresService proveedoresService,
                [FromBody] ProveedorRequestDto proveedorRequestDto)
        {
            if (proveedorRequestDto == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, "El cuerpo de la solicitud no puede ser nulo."));
            }

            try
            {
                var result = await proveedoresService.PutSaveProveedor(proveedorRequestDto);

                if (result == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, "Error al ejecutar el procedimiento almacenado."));
                }

                if (!result.Success)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, result.Message));
                }

                // Devolver todos los datos del proveedor
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, $"Error al actualizar el proveedor: {ex.Message}, InnerException: {ex.InnerException?.Message}"));
            }
        }

        #endregion


    }
}