using Microsoft.AspNetCore.Components;
using Nubetico.Frontend.Components.Core.Shared;
using Nubetico.Shared.Enums.Core;
using Radzen;
using Microsoft.Extensions.Localization;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Radzen.Blazor;
using System.Reflection;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class ProyectoDetComponent : NbBaseComponent, IDisposable
    {
        #region INJECTS
        [Inject] private IStringLocalizer<SharedResources>? LocalizerServices { get; set; }
        [Inject] private ProjectServices? ProjectApiServices { get; set; }
        [Inject] private NotificationService? NotificationService { get; set; }

        #endregion

        #region PARAMETERS
        #pragma warning disable CS8618 
        [Parameter] public ProjectDataDto ProjectData { get; set; }
        #pragma warning restore CS8618

        [Parameter] public TipoEstadoControl StateForm { get; set; }
        #endregion

        #region FORM
        private RadzenTemplateForm<ProjectDataDto> ProjectForm;
        #endregion

        #region PROPERTYS
        private bool IsLoadData { get; set; } = true;
        private IEnumerable<ElementsDropdownForm> Types { get; set; } = [];
        private IEnumerable<ElementsDropdownForm> Branch { get; set; } = [];
        private IEnumerable<ElementsDropdownForm> Status { get; set; } = [];
        private IEnumerable<ElementsDropdownForm> Subdivision { get; set; } = [];
        private IEnumerable<ElementsDropdownForm> Users { get; set; } = [];
        #endregion

        #region METHODS
        #region LIFE CICLE BLAZOR
        protected override void OnInitialized()
        {
            ProjectApiServices!.ValidateForm += ValidateForm;
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadFormDataAsync();
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firsRender)
        {
            if (firsRender && StateForm == TipoEstadoControl.Alta)
            {
               ProjectForm.EditContext.Validate();
            }
        }

        public void Dispose()
        {
            ProjectApiServices!.ValidateForm -= ValidateForm;
        }

        #endregion

        #region METHODS FORM
        private async Task LoadFormDataAsync()
        {
            var result = await ProjectApiServices!.GetProjectFormDataAsync();
            if (result == null || result.StatusCode > 300 || !result.Success || result.Data is not ProjectFormDataDto)
            {
                return;
            }

            try
            {
                var formData = result.Data!;
                Subdivision = formData.Subdivision;
                Types = formData.Types;
                Branch = formData.Branch;
                Users = formData.Users;
                Status = formData.Status;

                var usuario = await GetUsuarioAutenticadoAsync();
                ProjectData!.ActionUserGuid = Guid.TryParse(usuario?.FindFirst("id")?.Value, out Guid guid) ? guid : null;
            }
            catch (Exception ex) { return; }
        }

        private bool ValidateForm()
        {
            if (!ProjectForm!.EditContext.Validate())
            {
                NotifyAcces("Error al intentar guardar el proyecto", "Datos del proyecto incompletos", NotificationSeverity.Error);
                return false;
            }

            if (ProjectData.ProjectedStartDate > ProjectData.ProjectedEndDate)
            {
                NotifyAcces("Error al intentar guardar el proyecto", "La fecha de inicio proyectada no puede ser mayor a la fecha final proyectada", NotificationSeverity.Error);
                return false;
            }

            return true;
        }
        #endregion

        #region UTILS METHODS
        private void NotifyAcces(string summary, string details, NotificationSeverity severity) => NotificationService!.Notify(new() {
                Severity = severity,
                Summary = summary,
                Detail = details
            });

        private bool IsDisabledForm() => StateForm == TipoEstadoControl.Lectura;
        #endregion
        #endregion
    }
}
