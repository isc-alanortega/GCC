using FluentValidation;
using Microsoft.Extensions.Localization;
using Nubetico.Shared.Dto.ProyectosConstruccion.ReportesDestajos;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Validators
{
    public class ReporteDestajoDtoValidator : AbstractValidator<ReporteDestajoDto>
    {
        public ReporteDestajoDtoValidator(IStringLocalizer<SharedResources> localizer)
        {
            //RuleFor(m => m.Partidas).NotNull().NotEmpty().WithName(localizer["Core.Users.Username"]);
            //RuleFor(m => m.Password).NotNull().NotEmpty().WithName(localizer["Core.Users.Password"]);
        }
    }
}
