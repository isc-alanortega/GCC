using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.WebAPI.Filters;
using Nubetico.WebAPI.Application.Utils;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services;

namespace Nubetico.WebAPI.Controllers.ProyectosConstruccion
{
	[ApiController]
	[Authorize]
	[Route("api/v1/proyectosconstruccion/fraccionamientos")]
	[TypeFilter(typeof(ExceptionFilter))]
	public class SubdivisionsController : ControllerBase
	{
		[HttpGet("paginado")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<PaginatedListDto<SubdivisionsDto>>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
		public async Task<IActionResult> GetPaginatedSubdivisions([FromServices] SubdivisionsService subdivisionsService, [FromQuery] int limit, [FromQuery] int offset, [FromQuery] string? filtername)
		{
			if (limit < 0 || offset < 0)
			{
				return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, ""));
			}

			var result = await subdivisionsService.GetPaginatedSubdivisions(limit, offset, filtername);

			return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, result, ""));
		}

		[HttpGet("listado")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<IEnumerable<KeyValuePair<int, string>>>))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
		public async Task<IActionResult> GetSubdivisionsList([FromServices] SubdivisionsService subdivisionsService)
		{
			try
			{
				var result = await subdivisionsService.GetSubdivisionsKeyValue();
				return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, result, ""));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, ex.Message));
			}
			
		}

        [HttpGet("listado_etapas")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<IEnumerable<KeyValuePair<int, string>>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetStagesList([FromServices] SubdivisionsService subdivisionsService, [FromQuery] int? subdivisionid)
        {
            try
            {
                var result = await subdivisionsService.GetStagesKeyValue(subdivisionid);
                return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, result, ""));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, ex.Message));
            }

        }

        [HttpGet("listado_manzanas")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<IEnumerable<KeyValuePair<int, string>>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetBlocksList([FromServices] SubdivisionsService subdivisionsService, [FromQuery] int? subdivisionid, [FromQuery] int? stageid)
        {
            try
            {
                var result = await subdivisionsService.GetBlocksKeyValue(subdivisionid, stageid);
                return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, result, ""));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, ex.Message));
            }

        }

        [HttpGet("{uuid}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<SubdivisionsDto>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
		public async Task<IActionResult> GetSubdivisionByGuid([FromRoute] string uuid, [FromServices] SubdivisionsService subdivisionsService)
		{
			if (!Guid.TryParse(uuid, out var id))
			{
				return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest));
			}

			var subdivision = await subdivisionsService.GetSubdivisionByGuid(id);
			if (subdivision == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));
			}

			return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, subdivision));
		}

		[HttpPost("postfraccionamiento")]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponseDto<object>))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
		public async Task<IActionResult> PostSubdivision([FromBody] SubdivisionsDto subdivision, [FromServices] SubdivisionsService subdivisionsService)
		{
			try
			{
				var guidUser = HttpContext.User.Claims.FirstOrDefault(user => user.Type == "id")?.Value;
				string? resultUUID = await subdivisionsService.PostSubdivision(subdivision, guidUser ?? "");

				return StatusCode(StatusCodes.Status201Created, ResponseService.Response(StatusCodes.Status201Created, "Fraccionamiento guardado exitosamente", resultUUID));
			}
			catch(Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, ex.Message));
			}
		}

		[HttpPost("updatefraccionamiento")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<object>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
		public async Task<IActionResult> UpdateSubdivision([FromBody] SubdivisionsDto subdivision, [FromServices] SubdivisionsService subdivisionsService)
		{
			try
			{
				var guidUser = HttpContext.User.Claims.FirstOrDefault(user => user.Type == "id")?.Value;
				await subdivisionsService.UpdateSubdivision(subdivision, guidUser ?? "");

				return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, "Fraccionamiento guardado exitosamente"));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, ex.Message));
			}
		}
	}
}
