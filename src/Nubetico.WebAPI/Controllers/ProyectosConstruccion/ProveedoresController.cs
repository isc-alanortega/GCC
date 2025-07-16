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
        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<int>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
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

                if (!result.bResult)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, result.vchMessage));
                }

                // Devolver el ID del proveedor creado como parte de la respuesta
                return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result.IdProveedor, result.vchMessage));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, ex.Message));
            }
        }

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
        [HttpGet("{id}")]
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
    }
}