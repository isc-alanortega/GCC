using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.ProjectSectionDetails;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services.Projects;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services.ProjectSectionDetails;
using Nubetico.WebAPI.Application.Utils;
using Nubetico.WebAPI.Filters;
using System.Collections.Generic;

namespace Nubetico.WebAPI.Controllers.ProyectosConstruccion
{
    [ApiController]
    [Authorize]
    [Route("api/v1/proyectosconstruccion/sections-details")]
    [TypeFilter(typeof(ExceptionFilter))]
    public class ProjectSectionDetailsController : Controller
    {
        #region HTTP-GET
        [HttpGet("filter-section")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ResponseSectionDetailsDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetFilterSectionDetails(
            [FromServices] ProjectSectionDetailsService sectionService,
            [FromQuery] Guid sectionGuid
        )
        {
            var response = await sectionService.GetSectionDetailsByGuidAsync(sectionGuid);
            if (!response.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: response.Message));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<ResponseSectionDetailsDto>(StatusCodes.Status200OK, data: response.Result));
        }
        #endregion

        #region HTTP-POST
        [HttpPost("paginated-section")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<PaginatedListDto<SectionDetailsDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetFilterSectionDetails(
            [FromServices] ProjectSectionDetailsService sectionService,
            [FromBody] RequestSectionDetailsCatDto request
        )
        {
            var response = await sectionService.GetSectionDetailsPaginatedListAsync(request);
            if (!response.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: response.Message));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<PaginatedListDto<SectionDetailsDto>>(StatusCodes.Status200OK, data: response.Result));
        }
        #endregion
    }
}
