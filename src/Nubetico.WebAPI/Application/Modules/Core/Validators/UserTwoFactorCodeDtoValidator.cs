using FluentValidation;
using Nubetico.Shared.Dto.Core;

namespace Nubetico.WebAPI.Application.Modules.Core.Validators
{
    public class UserTwoFactorCodeDtoValidator : AbstractValidator<UserTwoFactorCodeDto>
    {
        public UserTwoFactorCodeDtoValidator()
        {
            RuleFor(m => m.Code).NotNull().NotEmpty();
            RuleFor(m => m.Key).NotNull().NotEmpty();
        }
    }
}
