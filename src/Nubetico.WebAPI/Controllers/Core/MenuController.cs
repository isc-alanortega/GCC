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
    [Route("api/v1/core/menu")]
    public class MenuController : ControllerBase
    {
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<List<MenuDto>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> GetAllAsync([FromServices] MenusService service)
        {
            var listaMenu = await service.GetAllMenusAsync();
            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, listaMenu));
        }

        [HttpGet("user")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<List<MenuUsuarioDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> GetUserMenusAsync([FromServices] MenusService service)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(user => user.Type == "id")?.Value;

            if (!Guid.TryParse(userId, out var guidUser))
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest));

            var listaMenu = await service.GetUserMenusAsync(guidUser);
            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, listaMenu));
        }

        [HttpGet("all-permissions")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<List<MenuPermisosDto>>))]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(BaseResponseDto<List<MenuPermisosDto>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> GetAllMenusPermissionsAsync([FromServices] MenusService service)
        {
            List<MenuPermisosDto>? listaMenusPermisos = await service.GetAllMenusPermissionsAsync();

            if(listaMenusPermisos == null)
                return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status204NoContent, new List<MenuPermisosDto>()));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, listaMenusPermisos));
        }
    }
}
