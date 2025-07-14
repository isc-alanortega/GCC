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
	[Route("api/v1/proyectosconstruccion/lotes")]
	[TypeFilter(typeof(ExceptionFilter))]
	public class LotsController : ControllerBase
	{
		[HttpPost("paginado")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<PaginatedListDto<LotsDto>>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
		public async Task<IActionResult> GetPaginatedSubdivisions([FromQuery] int limit, [FromQuery] int offset, [FromBody] FilterLots filter, [FromServices] LotsService lotsServices)
		{
			if (limit < 0 || offset < 0)
			{
				return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, ""));
			}

			var result = await lotsServices.GetPaginatedLots(limit, offset, filter);

			return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, result, ""));
		}

		[HttpGet("{uuid}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<LotsDetail>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
		public async Task<IActionResult> GetSubdivisionByGuid([FromRoute] string uuid, [FromServices] LotsService lotsServices)
		{
			if (!Guid.TryParse(uuid, out var id))
			{
				return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest));
			}

			var subdivision = await lotsServices.GetLotByGuid(id);
			if (subdivision == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));
			}

			return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, subdivision));
		}

		[HttpGet("listado_modelos")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<IEnumerable<KeyValuePair<int, string>>>))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
		public async Task<IActionResult> GetModelosList([FromServices] LotsService lotsServices)
		{
			try
			{
				var result = await lotsServices.GetModelsKeyValue();
				return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, result, ""));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, ex.Message));
			}

		}

		[HttpPost("postlote")]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponseDto<object>))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
		public async Task<IActionResult> PostSubdivision([FromBody] LotsDetail lot, [FromServices] LotsService lotsServices)
		{
			try
			{
				var guidUser = HttpContext.User.Claims.FirstOrDefault(user => user.Type == "id")?.Value;
				string? resultUUID = await lotsServices.PostLot(lot, guidUser ?? "");

				return StatusCode(StatusCodes.Status201Created, ResponseService.Response(StatusCodes.Status201Created, "Lote guardado exitosamente", resultUUID));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ResponseService.Response<object>(StatusCodes.Status500InternalServerError, ex.Message));
			}
		}

		[HttpPost("updatelote")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<object>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
		public async Task<IActionResult> UpdateSubdivision([FromBody] LotsDetail lot, [FromServices] LotsService lotsService)
		{
			try
			{
				var guidUser = HttpContext.User.Claims.FirstOrDefault(user => user.Type == "id")?.Value;
				await lotsService.UpdateLot(lot, guidUser ?? "");

				return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, "Lote guardado exitosamente"));
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, ex.Message));
			}
		}

        [HttpGet("check-block-in-lot")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> CheckBlockInLotById([FromQuery] int blockId, [FromServices] LotsService lotsService)
        {
            var respone = await lotsService.CheckBlockInLotsByIdAsync(blockId);
            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, data: respone));
        }

        [HttpGet("check-stage-in-lot")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> CheckStagekInLotById([FromQuery] int stageId, [FromServices] LotsService lotsService)
        {
            var respone = await lotsService.CheckStageInLotsByIdAsync(stageId);
            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, data: respone));
        }
    }
}
