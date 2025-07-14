using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.WebAPI.Application.Utils;
using Nubetico.WebAPI.Filters;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services;
using Nubetico.Shared.Dto.ProyectosConstruccion.Models;

namespace Nubetico.WebAPI.Controllers.ProyectosConstruccion
{
    [ApiController]
    [Authorize]
    [Route("api/v1/proyectosconstruccion/models")]
    [TypeFilter(typeof(ExceptionFilter))]
    public class ModelsController : Controller
    {
        [HttpGet("paginado")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<PaginatedListDto<InsumosDto>?>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetPaginatedModel(
             [FromServices] ModelsService modelsService,
             [FromQuery] int limit,
             [FromQuery] int offset,
             [FromQuery] string? name
         )
        {
            if (limit < 0 || offset < 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: ""));
            }

            var result = await modelsService.GetPaginatedModel(limit, offset, name);
            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<PaginatedListDto<ModelGridDto>?>(StatusCodes.Status200OK, data: result));
        }

        /// <summary>
        /// Retrieves the form data required.
        /// </summary>
        /// <param name="projectService">The service used to fetch the project form options.</param>
        /// <returns>Returns a response containing project form data.</returns>
        //[HttpGet("form")]
        //[Produces("application/json")]
        ////[ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client)]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ModelFetcherDto>))]
        //[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        //public async Task<IActionResult> GetProjectsFormData([FromServices] ModelsService modelsService)
        //{
        //    var response = await modelsService.GetFetchForm();
        //    return StatusCode(StatusCodes.Status200OK, ResponseService.Response<ModelFetcherDto>(StatusCodes.Status200OK, data: response));
        //}

        [HttpGet("found")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ModelDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetModel([FromServices] ModelsService modelsService, [FromQuery] int modelId)
        {
            var result = await modelsService.GetModelByIdAsync(modelId);
            if (result == null || !result.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: result!.Message));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<ModelDto>(StatusCodes.Status200OK, result.Result));
        }

        [HttpPost("add")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponseDto<ModelDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> PostAddModel([FromServices] ModelsService modelsService, [FromBody] ModelDto request)
        {
            var result = await modelsService.AddAsync(request);

            if (result == null || !result.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: result?.Message));

            return StatusCode(StatusCodes.Status201Created, ResponseService.Response<ModelDto>(StatusCodes.Status201Created, data: result.Result));
        }

        [HttpPatch("edit")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ModelDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> PatchEditModel([FromServices] ModelsService modelsService, [FromBody] ModelDto request)
        {
            var result = await modelsService.EditAsync(request);
            if (result == null || !result.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: result?.Message));

            return StatusCode(StatusCodes.Status201Created, ResponseService.Response<object>(StatusCodes.Status201Created, data: result.Result));
        }

        [HttpPatch("delete")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ModelDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> PatchDeleteModel([FromServices] ModelsService modelsService, [FromQuery] int modelId, [FromQuery] Guid userGuid)
        {
            var result = await modelsService.DeleteAsync(modelId, userGuid);
            if (result == null || !result.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: result?.Message));

            return StatusCode(StatusCodes.Status201Created, ResponseService.Response<object>(StatusCodes.Status201Created, data: result.Result));
        }
    }
}
