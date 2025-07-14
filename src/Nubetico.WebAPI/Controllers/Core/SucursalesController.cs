using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.WebAPI.Application.Modules.Core.Services;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Controllers.Core
{
    [ApiController]
    [Authorize]
    [Route("api/v1/core/sucursales")]
    public class SucursalesController : Controller
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<IEnumerable<SucursalDto>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> GetSucursales([FromServices] SucursalesService service)
        {
            var result = await service.GetSucursalesAsync();

            if(result == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result));
        }
    }
}
