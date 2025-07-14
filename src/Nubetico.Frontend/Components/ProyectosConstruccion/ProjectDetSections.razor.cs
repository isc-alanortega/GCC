using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Nubetico.Shared.Enums.Core;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class ProjectDetSections
    {
        #region INEJECTIONS
        [Inject] private IStringLocalizer<SharedResources>? LocalizerServices { get; set; }
        [Inject] private ProjectSectionService? SectionApiServices { get; set; }
        [Inject] private NotificationService? NotificationService { get; set; }
        [Inject] private DialogService? DialogService { get; set; }

        #endregion

        #region PARAMETERS
        [Parameter] public ProjectSectionDataDto SectionData { get; set; }
        [Parameter] public TipoEstadoControl? ActionForm { get; set; }
        [Parameter] public bool? IsDialogOrigen { get; set; } = false;
        [Parameter] public int SubdivisionId { get; set; }
        #endregion

        #region FORM
        private RadzenTemplateForm<ProjectSectionDataDto> SectionForm;
        #endregion

        #region PROPERTYS
        private bool IsSavingData { get; set; } = false;
        private IEnumerable<ElementsDropdownForm> StatusList { get; set; } = [];
        private IEnumerable<ElementsDropdownForm> GeneralContractorList { get; set; } = [];
        private IEnumerable<ElementsDropdownForm> Models { get; set; } = [];

        #endregion

        #region DATA GRID LOTS PROPERTYS
        public bool IsLoadData { get; set; } = false;
        private int LotCount { get; set; }
        private RadzenDataGrid<SectionLotsGridDto>? LotGrid { get; set; }
        public IList<SectionLotsGridDto> LotSelected { get; set; } = [];
        private List<SectionLotsGridDto> LotsData { get; set; } = [];
        private bool allowRowSelectOnRowClick = true;
        IEnumerable<int?> values = [];
        #endregion

        #region LIFE CILCE BLAZOR
        protected override async Task OnInitializedAsync()
        {
            await LoadFormDataAsync();

            //if(ActionForm != TipoEstadoControl.Alta) OnAutoSelectedRelationShip();

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firsRender)
        {
            if (firsRender && ActionForm == TipoEstadoControl.Alta)
            {
                SectionForm.EditContext.Validate();
            }
        }
        #endregion

        private void OnCheckboxChange(SectionLotsGridDto lot, bool? isSelected)
        {
            lot.IsLotSelected = isSelected ?? false;
            LotsData = LotsData.ToList(); // Clona para asegurar integridad

            LotGrid?.Reload();
            StateHasChanged();
        }

        private void HandleSaveSectionLotsSelected()
        {
            SectionData.SectionLots = LotsData.Where(item => item.LotId.HasValue && values.Contains(item.LotId.Value)).ToList();
            //    var lotsSelected = LotsData!.Where(item => item.IsLotSelected == true);
            //    SectionData!.SectionLots = lotsSelected.ToList();
        }

        #region SAVE FORM

        private bool ValidateForm()
        {

            if (!SectionForm!.EditContext.Validate())
            {
                NotifyAcces("Error al intentar guardar el la sección", "Datos de la sección incompletos", NotificationSeverity.Error);
                return false;
            }

            if (SectionData!.ProjectedStartDate > SectionData.ProjectedEndDate)
            {
                NotifyAcces("Error al intentar guardar el proyecto", "La fecha de inicio proyectada no puede ser mayor a la fecha final proyectada", NotificationSeverity.Error);
                return false;
            }

            return true;
        }

        private /*async*/ void FactoryTypeSaveWithDialog()
        {
            if (!ValidateForm()) return;

            int? projectId = GetProjectID();
            //if (projectId != null && projectId > 0 && IsDialogOrigen != true)
            //{
            //    await OnClickSaveAsync();
            //    DialogService!.Close();
            //    return;
            //}
            if(ActionForm == TipoEstadoControl.Alta)
                SectionData!.SectionGuid = Guid.NewGuid();
            
            HandleSaveSectionLotsSelected();


            DialogService!.Close(SectionData);
        }

        private async Task OnClickSaveAsync()
        {
            try
            {
                if (IsSavingData || SectionData?.ProjectId == null) return;

                IsSavingData = true;

                var result = await (ActionForm switch
                {
                    TipoEstadoControl.Alta => SectionApiServices!.PostAddSectionAsync(SectionData),
                    //ActionFormDto.Edit => ProjectApiServices.PatchEditProjectAsync(project: ProjectData),
                    _ => Task.FromResult<BaseResponseDto<ProjectSectionDataDto>>(null)
                });

                if (result.StatusCode == 500) return;

                var severity = result.StatusCode > 300 ? NotificationSeverity.Error : NotificationSeverity.Success;
                NotifyAcces("titulo", result.Message, severity);

                if (ActionForm == TipoEstadoControl.Alta)
                    SectionData!.Folio = result?.Data?.Folio;

                ActionForm = TipoEstadoControl.Lectura;
                this.EstadoControl = TipoEstadoControl.Lectura;
            }
            catch (Exception ex) 
            { 
            }

            IsSavingData = false;
        }
        #endregion

        #region METHODS BUTTON LIST 
        private void OnClickCancel()
        {
            this.EstadoControl = TipoEstadoControl.Lectura;
            ActionForm = TipoEstadoControl.Lectura;
        }

        private void OnClickClose()
        {
            this.CerrarTabNubetico();
            this.EstadoControl = TipoEstadoControl.Lectura;
            ActionForm = TipoEstadoControl.Lectura;
        }

        #endregion

        #region METHODS FORM
        private async Task LoadFormDataAsync()
        {
            try
            {
                var usuario = await GetUsuarioAutenticadoAsync();
                SectionData!.UserActionGuid = Guid.TryParse(usuario?.FindFirst("id")?.Value, out Guid guid) ? guid : null;

                var resultForm = await SectionApiServices!.GetSectionFormDataAsync(SubdivisionId, SectionData?.SectionId);
                if(resultForm != null && resultForm.Success && resultForm.Data != null && LotsData.Count <= 0)
                {
                    LotsData = resultForm.Data.LotsList.Data;
                    StatusList = resultForm.Data.SectionStatusList;
                    GeneralContractorList =  resultForm.Data.GeneralContractorsList;
                    Models = resultForm.Data.Models;
                }

            }
            catch (Exception ex)
            {
                string x = ex.Message;
            }
        }
        #endregion

        #region UTILS
        private void NotifyAcces(string summary, string details, NotificationSeverity severity) => NotificationService!.Notify(new()
        {
            Severity = severity,
            Summary = summary,
            Detail = details
        });

        private int? GetProjectID() => SectionData!.ProjectId;

        private bool IsDisabledForm() => ActionForm == TipoEstadoControl.Lectura;
        #endregion
    }
}
