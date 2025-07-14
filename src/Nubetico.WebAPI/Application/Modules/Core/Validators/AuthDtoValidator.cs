using FluentValidation;
using Microsoft.Extensions.Localization;
using Nubetico.Shared.Dto.Core;

namespace Nubetico.WebAPI.Application.Modules.Core.Validators
{
    public class AuthDtoValidator : AbstractValidator<AuthRequestDto>
    {
        public AuthDtoValidator(IStringLocalizer<SharedResources> localizer)
        {
            RuleFor(m => m.Username).NotNull().NotEmpty().WithName(localizer["Core.Users.Username"]);
            RuleFor(m => m.Password).NotNull().NotEmpty().WithName(localizer["Core.Users.Password"]);
        }
    }
}
