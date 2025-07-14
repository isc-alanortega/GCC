using FluentValidation;
using Microsoft.Extensions.Localization;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;

namespace Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Validators.Project
{
    public class ProjectDtoValidator : AbstractValidator<ProjectDataDto>
    {
        public ProjectDtoValidator(IStringLocalizer<SharedResources> localizer)
        {
            RuleFor(p => p.Name).NotNull().NotEmpty().WithName(localizer["ProyectosConstruccion.Projects.ErrorValidator.Name"]);
            RuleFor(p => p.BranchId).NotNull().WithName(localizer["ProyectosConstruccion.Projects.ErrorValidator.Branch"]);
            RuleFor(p => p.SubdivisionId).NotNull().WithName(localizer["ProyectosConstruccion.Projects.ErrorValidator.Subdivision"]);
            RuleFor(p => p.TypeId).NotNull().WithName(localizer["ProyectosConstruccion.Projects.ErrorValidator.Type"]);
            RuleFor(p => p.TotalUnits).NotNull().WithName(localizer["ProyectosConstruccion.Projects.ErrorValidator.TotalUnits"]);
            RuleFor(p => p.StatusId).NotNull().WithName(localizer["ProyectosConstruccion.Projects.ErrorValidator.Status"]);
            RuleFor(p => p.ResponsibleId).NotNull().WithName(localizer["ProyectosConstruccion.Projects.ErrorValidator.InCharge"]);
            RuleFor(p => p.ProjectedStartDate).NotNull().WithName(localizer["ProyectosConstruccion.Projects.ErrorValidator.ProjectedStartDate"]);
            RuleFor(p => p.ProjectedEndDate).NotNull().WithName(localizer["ProyectosConstruccion.Projects.ErrorValidator.ProjectedEndDate"]);
            RuleFor(p => p.ActionUserGuid).NotNull().WithName(localizer["Core.Error.UnknowError"]);

            // Validating the sections list
            RuleForEach(p => p.ProjectSectionData) // Applies the validation to each item in the Sections list
            .ChildRules(sections =>
            {
                sections.RuleFor(s => s.Name).NotNull().NotEmpty().WithName(localizer["ProyectosConstruccion.ProjectSection.ErrorValidator.Name"]);
                sections.RuleFor(s => s.Sequence).NotNull().WithName(localizer["Core.Error.UnknowError"]);
                sections.RuleFor(s => s.Status).NotNull().WithName(localizer["ProyectosConstruccion.ProjectSection.ErrorValidator.Status"]);
                sections.RuleFor(s => s.GeneralContractor).NotNull().WithName(localizer["ProyectosConstruccion.ProjectSection.ErrorValidator.GeneralContractor"]);
                sections.RuleFor(s => s.ProjectedStartDate).NotNull().WithName(localizer["ProyectosConstruccion.ProjectSection.ErrorValidator.ProjectedStartDate"]);
                sections.RuleFor(s => s.ProjectedEndDate).NotNull().WithName(localizer["ProyectosConstruccion.ProjectSection.ErrorValidator.ProjectedEndDate"]);
                
                // Validating the phases list
                sections.RuleForEach(s => s.ProjectSectionPhase) // Applies the validation to each item in the Phases list
                .ChildRules(phases =>
                {
                    phases.RuleFor(s => s.Name).NotNull().NotEmpty().WithName(localizer["ProyectosConstruccion.ProjectSectionPhase.ErrorValidator.Name"]);
                    phases.RuleFor(s => s.SectionGuidTemp).NotNull().NotEmpty().WithName(localizer["Core.Error.UnknowError"]);
                    phases.RuleFor(s => s.Sequence).NotNull().WithName(localizer["Core.Error.UnknowError"]);
                });
            });
        }
    }
}
