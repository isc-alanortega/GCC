using FluentValidation;
using Microsoft.Extensions.Localization;
using Nubetico.Shared.Dto.Core;

namespace Nubetico.WebAPI.Application.Modules.Core.Validators
{
    public class UsuarioNubeticoDtoValidator: AbstractValidator<UsuarioNubeticoDto>
    {
        public UsuarioNubeticoDtoValidator(IStringLocalizer<SharedResources> localizer)
        {
            RuleFor(m => m.Username).NotNull().NotEmpty().MaximumLength(256).WithName(localizer["Core.Users.Username"]);
            RuleFor(m => m.Email).NotNull().NotEmpty().MaximumLength(320).WithName(localizer["Core.Users.Email"]);
            RuleFor(m => m.Nombres).NotNull().NotEmpty().MaximumLength(100).WithName(localizer["Core.Users.FirstName"]);
            RuleFor(m => m.PrimerApellido).NotNull().NotEmpty().MaximumLength(100).WithName(localizer["Core.Users.LastName"]);
            RuleFor(m => m.SegundoApellido).MaximumLength(100).WithName(localizer["Core.Users.SecondLastName"]);
            //RuleFor(m => m.Habilitado).NotNull();
        }
    }
}
