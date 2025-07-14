using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Nubetico.Shared.Enums.Core;
using Radzen;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class ProjectDetPhase
    {
        #region INEJECTIONS
        [Inject] private IStringLocalizer<SharedResources>? LocalizerServices { get; set; }
        [Inject] private ProjectServices ProjectApiServices { get; set; }
        [Inject] private NotificationService NotificationService { get; set; }
        [Inject] private DialogService DialogService { get; set; }

        #endregion

        #region PARAMETERS
        [Parameter] public ProjectSectionPhaseDto? PhaseData { get; set; }
        [Parameter] public bool? IsDialogOrigen { get; set; } = false;
        [Parameter] public TipoEstadoControl ActionForm { get; set; }
        #endregion


        #region METHODS FORM
        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(PhaseData?.Name))
            {
                NotifyAcces("Error al intentar guardar el proyecto", "El nombre del proyecto es requerido", NotificationSeverity.Error);
                return false;
            }

            if (string.IsNullOrEmpty(PhaseData?.Description))
            {
                NotifyAcces("Error al intentar guardar el proyecto", "La descripción es requerida", NotificationSeverity.Error);
                return false;
            }

            return true;
        }

        private async Task FactoryTypeSaveWithDialogAsync()
        {
            if (!ValidateForm()) return;

            if (IsDialogOrigen != true)
            {
                await Task.CompletedTask;
            }

            DialogService.Close(PhaseData);
        }



        private IEnumerable<ElementsDropdownForm> StatusList { get; set; } = [
            new() { Id = 1, Name ="Pendiente" },
            new() { Id = 2, Name ="En proceso" },
            new() { Id = 3, Name ="En riesgo" },
            new() { Id = 4, Name ="Requiere actualización" },
            new() { Id = 5, Name ="Detenido" },
            new() { Id = 6, Name ="Completado" },
            new() { Id = 7, Name ="Cancelado" }
        ];

        private void NotifyAcces(string summary, string details, NotificationSeverity severity) => NotificationService!.Notify(new()
        {
            Severity = severity,
            Summary = summary,
            Detail = details
        });

        private bool GetIsDisabled() => ActionForm == TipoEstadoControl.Lectura;
        #endregion
    }
}
