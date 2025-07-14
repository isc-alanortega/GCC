using Microsoft.AspNetCore.Mvc;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.WebAPI.Application.Modules.Core.Services;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Controllers.Core
{
    [ApiController]
    [Route("api/v1/core/customization")]
    public class CustomizationController : ControllerBase
    {
        [HttpGet("tema")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ThemeConfigDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> GetTema([FromServices] ParametrosService parametrosService)
        {
            var tema = await parametrosService.GetTheme();

            if (tema == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound, null, ""));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, tema, ""));
        }
    }
}
