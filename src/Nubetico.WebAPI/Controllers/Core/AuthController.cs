using FluentValidation;
using FluentValidation.Results;
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
    [Route("api/v1/core/auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<AuthResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<List<ValidationFailure>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> Autenticar(
            [FromServices] UsuariosService usuariosService,
            [FromBody] AuthRequestDto authDto,
            [FromServices] IValidator<AuthRequestDto> validator)
        {
            var validate = await validator.ValidateAsync(authDto);

            if (!validate.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response(StatusCodes.Status400BadRequest, validate.Errors));

            var authResult = await usuariosService.GetPerfilUsuarioPorAutenticacionAsync(authDto);

            if (authResult.Item1 == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound, null, authResult.Item2));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, authResult.Item1, authResult.Item2));
        }

        [Authorize]
        [HttpGet("renew-token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<AuthResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<List<ValidationFailure>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> RenewToken([FromServices] UsuariosService usuariosService)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(user => user.Type == "id")?.Value;

            if (!Guid.TryParse(userId, out var guidUser))
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest));

            var tokenData = await usuariosService.GetTokenByUserGuidAsync(guidUser);

            if (tokenData == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound, null));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, tokenData));
        }

        [HttpPost("verify-token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> VerifyToken([FromServices] UsuariosService usuariosService, [FromBody] VerifyTokenRequestDto verifyTokenRequestDto)
        {
            if (string.IsNullOrEmpty(verifyTokenRequestDto.Token))
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest));

            if (!Guid.TryParse(verifyTokenRequestDto.Token, out var guidToken))
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest));

            bool? tokenValid = await usuariosService.GetTokenIsValidAsync(guidToken);

            if (tokenValid == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, tokenValid));
        }

        [HttpPost("update-auth")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<>))]
        public async Task<IActionResult> UpdatePasswordByToken([FromServices] UsuariosService usuariosService,
            [FromBody] UpdatePswdByTokenDto updatePswdByTokenDto,
            [FromServices] IValidator<UpdatePswdByTokenDto> validator,
            [FromServices] IStringLocalizer<SharedResources> localizer)
        {
            var validate = await validator.ValidateAsync(updatePswdByTokenDto);

            if (!validate.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response(StatusCodes.Status400BadRequest, validate.Errors));

            bool? success = await usuariosService.SetUpdatePasswordByTokenAsync(updatePswdByTokenDto);

            if (success == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound, null, localizer["Core.Errors.TokenNotFoundOrExpired"]));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, success, localizer["Core.Users.PasswordUpdateSuccess"]));
        }
    }
}
