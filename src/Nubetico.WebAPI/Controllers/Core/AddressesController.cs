using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Nubetico.Shared.Dto.Common;
using Nubetico.WebAPI.Application.Modules.Core.Services;
using Nubetico.WebAPI.Application.Utils;
using Nubetico.Shared.Dto.Core;

namespace Nubetico.WebAPI.Controllers.Core
{
	[ApiController]
	[Authorize]
	[Route("api/v1/core/domicilios")]
	public class AddressesController : ControllerBase
	{
		[HttpGet("domiciliobyid")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<DomicilioDto>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
		public async Task<IActionResult> GetDomicilioByID([FromServices] DomiciliosService domiciliosService, [FromQuery] int id_domicilio)
		{
			var domicilio = await domiciliosService.GetDomicilioByID(id_domicilio);
			if (domicilio == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));
			}

			return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, domicilio));
		}

		[HttpGet("listado_estados")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<IEnumerable<KeyValuePair<int, string>>>))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
		public async Task<IActionResult> GetEstadosList([FromServices] DomiciliosService domiciliosService)
		{
			try
			{
				var result = await domiciliosService.GetEstadosKeyValueAsync();
				return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, result, ""));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, ex.Message));
			}
		}

		[HttpGet("listado_municipios")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<IEnumerable<TripletValueSAT>>))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
		public async Task<IActionResult> GetMunicipiosList([FromQuery] string? c_Estado, [FromQuery] string? c_Municipio, [FromServices] DomiciliosService domiciliosService)
		{
			try
			{
				var result = await domiciliosService.GetMunicipiosListAsync(c_Estado, c_Municipio);
				return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, result, ""));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, ex.Message));
			}
		}

		[HttpGet("listado_colonias")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<IEnumerable<TripletValueSAT>>))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
		public async Task<IActionResult> GetColoniasList([FromQuery] string? codigoPostal, [FromQuery] string? filtro, [FromServices] DomiciliosService domiciliosService)
		{
			try
			{
				var result = await domiciliosService.GetColoniasListAsync(codigoPostal, filtro);
				return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, result, ""));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, ex.Message));
			}
		}

		[HttpGet("codigopostal_informacion")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<CodigoPostalDto>))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
		public async Task<IActionResult> GetCodigoPostalInfo([FromQuery] string codigoPostal, [FromServices] DomiciliosService domiciliosService)
		{
			try
			{
				var result = await domiciliosService.GetCodigoPostalInfoAsync(codigoPostal);
				return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, result, ""));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, ex.Message));
			}
		}

		[HttpPost("postdomicilio")]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponseDto<object>))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
		public async Task<IActionResult> PostSubdivision([FromBody] DomicilioDto domicilio, [FromServices] DomiciliosService domiciliosService)
		{
			try
			{
				var guidUser = HttpContext.User.Claims.FirstOrDefault(user => user.Type == "id")?.Value;
				int? resultID = await domiciliosService.PostDomicilio(domicilio, guidUser ?? "");

				return StatusCode(StatusCodes.Status201Created, ResponseService.Response(StatusCodes.Status201Created, "Domicilio guardado exitosamente", resultID != null ? resultID.ToString() : null));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, ex.Message));
			}
		}
	}
}
