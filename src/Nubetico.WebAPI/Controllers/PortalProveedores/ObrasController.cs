using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.PortalProveedores;
using Nubetico.WebAPI.Application.Modules.PortalProveedores.Services;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Controllers.PortalProveedores
{
    [ApiController]
    [Authorize]
    [Route("api/v1/portalproveedores/obras")]
    public class ObrasController : ControllerBase
    {
        [HttpGet("paginado")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<PaginatedListDto<ObraDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetPaginadoAsync(
            [FromServices] ObrasService obrasService,
            [FromQuery] int limit,
            [FromQuery] int offset,
            [FromQuery] string? orderBy)
        {
            if (limit < 0 || offset < 0)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, ""));

            PaginatedListDto<ObraDto> result = await obrasService.GetObrasPaginado(limit, offset, null);

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, result, ""));
        }
    }
}
