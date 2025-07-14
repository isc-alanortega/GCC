using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Nubetico.Shared.Dto.ProyectosConstruccion.Sections;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services.Projects;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Controllers.ProyectosConstruccion
{
    [ApiController]
    [Authorize]
    [Route("api/v1/proyectosconstruccion/proyectos-secciones")]
    public class ProjectsSectionsController : Controller
    {
        [HttpGet("get-project-sections")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponseDto<IEnumerable<ProjectSectionDataDto>?>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetProjectsSectionsByProjectIdAsync(
            [FromServices] ProjectSectionService sectionService,
            [FromQuery] int projectId
        )
        {
            var response = await sectionService.FindSectionsByProjectIdAsync(projectId);
            if (!response.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: response.Message));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<IEnumerable<ProjectSectionDataDto>?>(StatusCodes.Status200OK, data: response.Result));
        }


        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponseDto<ProjectSectionDataDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> PostAddSection(
            [FromServices] ProjectSectionService sectionService,
            [FromBody] ProjectSectionDataDto request
        )
        {
            var response = await sectionService.AddSectionAsync(request);
            if (!response!.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: response.Message));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<ProjectSectionDataDto>(StatusCodes.Status200OK, data: response.Result));
        }

        [HttpPatch("delete-phase")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> PatchDeletePhase(
            [FromServices] ProjectSectionPhaseService phaseService,
            [FromQuery] int phaseId
        )
        {
            var response = await phaseService.DeletePhasesAsync(phaseId);
            if (!response)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: "No fue posible borrar la fase"));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<bool>(StatusCodes.Status200OK, data: response));
        }

        [HttpGet("get-form-data")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ProjectSectionFetcherDto?>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetSectionFormDataById(
           [FromServices] ProjectSectionService sectionService,
           [FromQuery] int subdivisionId,
           [FromQuery] int? sectionId
        )
        {
            var response = await sectionService.GetSectionFetchAsync(subdivisionId, sectionId);
            if (!response.Success || response.Result == null)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: response.Message));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<ProjectSectionFetcherDto?>(StatusCodes.Status200OK, data: response.Result));
        }

        [HttpGet("select")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<List<BasicItemSelectDto>>))]
        public async Task<IActionResult> GetProjectSectionSelectAsync([FromServices] ProjectSectionService service, [FromQuery] int? contractorId)
        {
            var result = await service.GetProjectSectionSelectDtoAsync(contractorId);

            if (result == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result));
        }
    }
}
