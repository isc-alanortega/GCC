using FluentValidation;
using Microsoft.Extensions.Localization;
using Nubetico.Shared.Dto.Core;

namespace Nubetico.WebAPI.Application.Modules.Core.Validators
{
    public class UpdatePswdByTokenDtoValidator : AbstractValidator<UpdatePswdByTokenDto>
    {
        public UpdatePswdByTokenDtoValidator(IStringLocalizer<SharedResources> localizer)
        {
            RuleFor(m => m.Pswd)
                .NotNull()
                .NotEmpty()
                .WithName(localizer["Core.Users.NewPassword"]);

            RuleFor(m => m.PswdConfirm)
                .NotNull()
                .NotEmpty()
                .WithName(localizer["Core.Users.ConfirmPassword"])
                .Equal(m => m.Pswd)
                .WithMessage(localizer["Core.Users.PasswordMismatch"]);
        }
    }

}
