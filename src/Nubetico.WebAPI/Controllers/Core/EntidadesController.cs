using Microsoft.AspNetCore.Mvc;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;
using Nubetico.WebAPI.Application.Modules.Core.Services;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Controllers.Core
{

    [Route("api/v1/core/entidades")]
    [ApiController]
    public class EntidadesController : ControllerBase
    {
        [HttpGet("GetAllTipoRegimen")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<List<TablaRelacionDto>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetAllTipoRegimenFiscal(
             [FromServices] EntidadesService entidadesService)
        {
            try
            {
                var result = await entidadesService.GetAllTipoRegimenFiscal();

                if (result == null || !result.Any())
                {
                    return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, new List<TablaRelacionDto>(), "No hay Tipos de Régimen registrados."));
                }

                return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result, "Tipos de Régimen encontrados."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, ex.Message));
            }
        }
        
        [HttpGet("GetAllRegimenFiscal")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<List<TablaRelacionDto>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetAllRegimenFiscal(
             [FromServices] EntidadesService entidadesService)
        {
            try
            {
                var result = await entidadesService.GetAllRegimenFiscal();

                if (result == null || !result.Any())
                {
                    return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, new List<TablaRelacionDto>(), "No hay Tipos de Régimen registrados."));
                }

                return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result, "Tipos de Régimen encontrados."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, ex.Message));
            }
        }
        [HttpGet("GetAllFormaPago")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<List<TablaRelacionDto>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetAllFormaPago(
             [FromServices] EntidadesService entidadesService)
        {
            try
            {
                var result = await entidadesService.GetAllFormaPago();

                if (result == null || !result.Any())
                {
                    return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, new List<TablaRelacionDto>(), "No hay Formas de Pagos registrados."));
                }

                return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result, "Formas de Pago encontrados."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, ex.Message));
            }
        }
        [HttpGet("GetAllMetodoDePago")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<List<TablaRelacionStringDto>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetAllMetodoDePago(
             [FromServices] EntidadesService entidadesService)
        {
            try
            {
                var result = await entidadesService.GetAllMetodoDePago();

                if (result == null || !result.Any())
                {
                    return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, new List<TablaRelacionStringDto>(), "No hay Métodos de Pago registrados."));
                }

                return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result, "Métodos de Pago encontrados."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, ex.Message));
            }
        }
        [HttpGet("GetAllUsoCFDI")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<List<TablaRelacionStringDto>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetAllUsoCFDI(
     [FromServices] EntidadesService entidadesService)
        {
            try
            {
                var result = await entidadesService.GetAllUsoCFDI();

                if (result == null || !result.Any())
                {
                    return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, new List<TablaRelacionStringDto>(), "No hay Usos de CFDI registrados."));
                }

                return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result, "Usos de CFDI encontrados."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, ex.Message));
            }
        }

    }

        //[ApiController]
        //[Authorize]
        //[Route("api/v1/core/entidades")]
        //[TypeFilter(typeof(ExceptionFilter))]
        //public class EntidadesController : ControllerBase
        //{
        //	[HttpGet("found")]
        //	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<SuppliesDto>))]
        //	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        //	[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        //	public async Task<IActionResult> GetSupplies([FromServices] SuppliesService suppliesService, [FromQuery] int suppliesId)
        //	{
        //		var result = await suppliesService.GetSuppliesByIdAsync(suppliesId);
        //		if (result == null || !result.Success)
        //			return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: result!.Message));

        //		return StatusCode(StatusCodes.Status200OK, ResponseService.Response<SuppliesDto>(StatusCodes.Status200OK, result.Result));
        //	}
        //}
    }
