using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.WebAPI.Application.Modules.Core.Services;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Controllers.Core
{
    [ApiController]
    [Authorize]
    [Route("api/v1/core/usuarios")]
    public class UsuariosController : ControllerBase
    {
        [HttpGet("paginado")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<PaginatedListDto<UsuarioNubeticoGridDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> GetUsuariosPaginado(
            [FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromQuery] string? orderBy,
            [FromQuery] string? nombreCompleto,
            [FromQuery] string? username,
            [FromQuery] int? idEstadoUsuario,
            [FromServices] UsuariosService usuariosService)
        {
            if (!limit.HasValue || !offset.HasValue)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest));

            var listaPaginada = await usuariosService.GetUsuariosPaginadoAsync(limit.Value, offset.Value, orderBy, username, nombreCompleto, idEstadoUsuario);

            if (listaPaginada.Data == null || listaPaginada.Data.Count.Equals(0))
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, listaPaginada));
        }

        [HttpGet("select-estados")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<BasicItemSelectDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> GetUserStatus([FromServices] UsuariosService usuariosService)
        {
            var result = await usuariosService.GetUserStatusListAsync();
            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result));
        }

        [HttpGet("{uuid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<UsuarioNubeticoDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> GetUsuarioByGuid(
            [FromRoute] string uuid,
            [FromServices] UsuariosService usuariosService)
        {
            if (!Guid.TryParse(uuid, out var id))
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest));

            var usuarioNubeticoDto = await usuariosService.GetUsuarioNubeticoByGuidAsync(id);

            if (usuarioNubeticoDto == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, usuarioNubeticoDto));
        }

        [HttpGet("username/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<UsuarioNubeticoDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> GetUsuarioByUsername(
            [FromRoute] string username,
            [FromServices] UsuariosService usuariosService)
        {
            var result = await usuariosService.GetUsuarioDirectoryByUsernameAsync(username);

            if (result == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result));
        }

        /// <summary>
        /// Insert de usuario
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponseDto<UsuarioNubeticoDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<>))]
        //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> PostUsuario(
            [FromBody] UsuarioNubeticoDto usuarioDto,
            [FromServices] UsuariosService usuariosService,
            [FromServices] IStringLocalizer<SharedResources> localizer,
            [FromServices] IValidator<UsuarioNubeticoDto> validator)
        {
            var validate = await validator.ValidateAsync(usuarioDto);

            if (!validate.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response(StatusCodes.Status400BadRequest, validate.Errors));

            var guidUser = HttpContext.User.Claims.FirstOrDefault(user => user.Type == "id")?.Value;
            UsuarioNubeticoDto result = await usuariosService.SetInsertUsuarioAsync(usuarioDto, guidUser ?? throw new Exception(localizer["Core.Errors.UserNotLogged"]), Request.Scheme);

            //if(result == null)
            //    return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));

            return StatusCode(StatusCodes.Status201Created, ResponseService.Response(StatusCodes.Status201Created, result));
        }

        /// <summary>
        /// Update usuario
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<UsuarioNubeticoDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> PutUsuario(
            [FromBody] UsuarioNubeticoDto usuarioDto,
            [FromServices] UsuariosService usuariosService,
            [FromServices] IValidator<UsuarioNubeticoDto> validator)
        {
            var validate = await validator.ValidateAsync(usuarioDto);

            if (!validate.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response(StatusCodes.Status400BadRequest, validate.Errors));

            var result = await usuariosService.SetUpdateUsuarioAsync(usuarioDto);

            if (result == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result));
        }

        [HttpGet("generar-qr")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<GeneratedTwoFactorCodeDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> GenerarQR([FromQuery] string guidUsuario, [FromQuery] bool newCode, [FromServices] UsuariosService usuariosService)
        {
            if (!Guid.TryParse(guidUsuario, out var id))
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest));

            var resultQr = await usuariosService.GetCodigoQrByGuidAsync(id, newCode);

            if (resultQr == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, resultQr));
        }

        /// <summary>
        /// Set user token
        /// </summary>
        /// <returns></returns>
        [HttpPost("set-token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<UserTwoFactorCodeDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> SetTokenUsuario(
            [FromBody] UserTwoFactorCodeDto userTwoFactorCodeDto,
            [FromServices] UsuariosService usuariosService,
            [FromServices] IValidator<UserTwoFactorCodeDto> validator)
        {
            var validate = await validator.ValidateAsync(userTwoFactorCodeDto);

            if (!validate.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response(StatusCodes.Status400BadRequest, validate.Errors));

            var result = await usuariosService.SetUserTokenByValidation(userTwoFactorCodeDto);

            if (result == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));

            return StatusCode(StatusCodes.Status201Created, ResponseService.Response(StatusCodes.Status200OK, result));
        }
    }
}
