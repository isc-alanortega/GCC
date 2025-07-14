using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Supplies;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services;
using Nubetico.WebAPI.Application.Utils;
using Nubetico.WebAPI.Filters;

namespace Nubetico.WebAPI.Controllers.ProyectosConstruccion
{
    [ApiController]
	[Authorize]
	[Route("api/v1/proyectosconstruccion/insumos")]
	[TypeFilter(typeof(ExceptionFilter))]
	public class SuppliesController : ControllerBase
	{
		[HttpGet("paginado")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<PaginatedListDto<InsumosDto>?>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetPaginatedSupplies(
            [FromServices] SuppliesService suppliesService,
            [FromQuery] int limit,
            [FromQuery] int offset,
            [FromQuery] string? code,
            [FromQuery] string? description,
            [FromQuery] int? typeId
        )
		{
            if (limit < 0 || offset < 0)
            {
				return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: ""));
			}
			
			var result = await suppliesService.GetPaginatedSupplies(limit, offset, code, description, typeId);
			return StatusCode(StatusCodes.Status200OK, ResponseService.Response<PaginatedListDto<InsumosDto>?>(StatusCodes.Status200OK, data: result));
		}

        /// <summary>
        /// Retrieves the form data required.
        /// </summary>
        /// <param name="projectService">The service used to fetch the project form options.</param>
        /// <returns>Returns a response containing project form data.</returns>
        [HttpGet("form")]
        [Produces("application/json")]
        //[ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<SuppliesFetcherDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetProjectsFormData([FromServices] SuppliesService suppliesService)
        {
            var response = await suppliesService.GetFetchForm();
            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<SuppliesFetcherDto>(StatusCodes.Status200OK, data: response));
        }

        [HttpGet("found")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<SuppliesDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetSupplies([FromServices] SuppliesService suppliesService, [FromQuery] int suppliesId)
        {
            var result = await suppliesService.GetSuppliesByIdAsync(suppliesId);
            if(result == null || !result.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: result!.Message));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<SuppliesDto>(StatusCodes.Status200OK, result.Result));
        }

        [HttpPost("add")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponseDto<SuppliesDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> PostAddSupplies([FromServices] SuppliesService suppliesService, [FromBody] SuppliesDto request)
        {
            var result = await suppliesService.AddAsync(request);

            if(result == null || !result.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: result?.Message));

            return StatusCode(StatusCodes.Status201Created, ResponseService.Response<SuppliesDto>(StatusCodes.Status201Created, data: result.Result));
        }

        [HttpPatch("edit")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<SuppliesDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> PatchEditSupplies([FromServices] SuppliesService suppliesService, [FromBody] SuppliesDto request)
        {
            var result = await suppliesService.EditAsync(request);
            if (result == null || !result.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: result?.Message));

            return StatusCode(StatusCodes.Status201Created, ResponseService.Response<object>(StatusCodes.Status201Created, data: result.Result));
        }

        [HttpPatch("delete")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<SuppliesDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]        
        public async Task<IActionResult> PatchDeleteSupplies([FromServices] SuppliesService suppliesService, [FromQuery] int suppliesId, [FromQuery] Guid userGuid)
        {
            var result = await suppliesService.DeleteAsync(suppliesId, userGuid);
            if (result == null || !result.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: result?.Message));

            return StatusCode(StatusCodes.Status201Created, ResponseService.Response<object>(StatusCodes.Status201Created, data: result.Result));
        }
    }
}
