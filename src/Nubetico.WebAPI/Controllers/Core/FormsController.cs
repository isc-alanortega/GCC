using Microsoft.AspNetCore.Mvc;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.WebAPI.Application.Modules.Core.Services;
using Nubetico.WebAPI.Application.Utils;
using Nubetico.WebAPI.Filters;

namespace Nubetico.WebAPI.Controllers.Core
{
    [Route("api/v1/core/forms")]
    [ApiController]
    public class FormsController : ControllerBase
    {
        [HttpGet("id/{alias}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<FormRequestDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> GetForm(
            [FromRoute] string alias,
            [FromServices] FormsService service)
        {
            if (string.IsNullOrEmpty(alias))
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest));

            var form = await service.GetFormByAliasAsync(alias);

            if (form == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, form));
        }

        [HttpPost]
        [ServiceFilter(typeof(TurnstileFilter))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> PostForm(
            [FromBody] FormPostDto formPostDto,
            [FromServices] FormsService service)
        {
            // Función para obtener IP desde el proxy inverso
            string ipAddress = NetworkUtils.GetClientIpAddress(HttpContext);

            var result = await service.PostFormAsync(formPostDto, ipAddress);

            if (result == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound, null, "Formulario no encontrado"));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result.Item1, result.Item2));
        }
    }
}
