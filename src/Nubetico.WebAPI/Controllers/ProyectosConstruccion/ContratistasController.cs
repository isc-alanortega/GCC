using Microsoft.AspNetCore.Mvc;
using Nubetico.DAL.ResultSets.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Contratistas;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Controllers.ProyectosConstruccion
{
    [Route("api/v1/proyectosconstruccion/contratistas")]
    [ApiController]
    public class ContratistasController : ControllerBase
    {



        #region POST
        [HttpPost("PostSaveContratista")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ContratistaResult>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> CreateProveedor(
    [FromServices] ContratistasService contratistasService,
    [FromBody] ContratistasDto contratistaRequest)
        {
            // Validar que el cuerpo de la solicitud no sea nulo
            if (contratistaRequest == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, "El cuerpo de la solicitud no puede ser nulo."));
            }

            try
            {
                var result = await contratistasService.CreateContratistaAsync(contratistaRequest);

                if (result == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, "Error al ejecutar el procedimiento almacenado."));
                }

                if (!result.Data.bResult)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, result.Data.vchMessage));
                }

                // Devolver todos los datos del proveedor creado
                return Ok(new BaseResponseDto<ContratistaResult>
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
        [HttpGet("GetContratistaPaginado")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<PaginatedListDto<ContratistaGridResultSet>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetContratistaPaginado(
                [FromQuery] int? limit,
                [FromQuery] int? offset,
                [FromQuery] string? orderBy,
                [FromQuery] string? nombre,
                [FromQuery] string? rfc,
                [FromServices] ContratistasService contratistasService)
        {
            try
            {
                if (!limit.HasValue || !offset.HasValue) return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, "Limit y offset son requeridos."));
                var listaPaginada = await contratistasService.GetContratistasPaginadoAsync(limit.Value, offset.Value, orderBy, nombre, rfc);
                if (listaPaginada.Data == null || !listaPaginada.Data.Data.Any()) return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound, null, "No se encontraron contratistas."));
                // Devolver directamente el BaseResponseDto del servicio
                return Ok(listaPaginada);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, ex.Message));
            }
        }

        [HttpGet("GetContratistaById{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ContratistasDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetContratistaById(
        [FromServices] ContratistasService contratistasService,
        [FromRoute] int id)
        {
            try
            {
                var result = await contratistasService.GetContratistaByIdAsync(id);

                if (result == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound, null, "Contratista no encontrado."));
                }

                return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result, "Contratista encontrado."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, ex.Message));
            }
        }
        #endregion
        #region PUT
        [HttpPut("PutSaveContratista")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ContratistaResult>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutSaveContratista(
    [FromServices] ContratistasService contratistasService,
    [FromBody] ContratistasDto contratistaRequestDto)
        {
            if (contratistaRequestDto == null)
            {
                return BadRequest(ResponseService.Response<object>(400, null, "El cuerpo de la solicitud no puede ser nulo."));
            }

            var result = await contratistasService.PutSaveContratista(contratistaRequestDto);

            if (!result.Success)
            {
                return StatusCode(400, result);
            }

            return Ok(result);
        }

        #endregion

    }
}
