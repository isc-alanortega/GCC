using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Nubetico.Frontend.Components.Core.Shared;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Models.Enums.Core;
using Nubetico.Frontend.Pages.ProyectosConstruccion;
using Nubetico.Frontend.Services.Core;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class ProyectoTabSeccionesComponent : NbBaseComponent, IDisposable
    {
        #region INEJECTIONS
        [Inject] private IStringLocalizer<SharedResources>? LocalizerServices { get; set; }
        [Inject] private ProjectServices? ProjectApiServices { get; set; }
        [Inject] private ProjectSectionService? SectionApiServices { get; set; }
        [Inject] private NotificationService? Notification { get; set; }
        [Inject] private DialogService? Dialog { get; set; }
        [Inject] private GlobalBreakpointService? BreakpointService { get; set; }
        #endregion

        #region PARAMETERS
        [Parameter] public TipoEstadoControl StateForm { get; set; }
        [Parameter] public ProjectDataDto? ProjectData { get; set; }
        #endregion

        #region PROPERTYS
        private bool IsLoadData { get; set; } = false;
        private IEnumerable<ElementsDropdownForm> Sections { get; set; } = [];
        private IEnumerable<ElementsDropdownForm> Phases { get; set; } = [];
        #endregion

        #region PROPERTYS GRID SECTIONS
        private int SectionsCount { get; set; }
        private RadzenDataGrid<ProjectSectionDataDto>? SectionsGrid { get; set; }
        public IList<ProjectSectionDataDto> SectionsSelected { get; set; } = [];
        #endregion

        #region PROPERTYS GRID PHASES
        private int PhasesCount { get; set; }
        private RadzenDataGrid<ProjectSectionPhaseDto>? PhasesGrid { get; set; }
        public IList<ProjectSectionPhaseDto> PhasesSelected { get; set; } = [];
        #endregion

        #region METHODS LIFE CICLE BLAZOR
        protected override void OnInitialized()
        {
            BreakpointService!.OnChange += StateHasChanged;
            ProjectApiServices!.RefreshGrid += RefreshGrid;
            base.OnInitialized();
        }        

        public void Dispose()
        {
            BreakpointService!.OnChange -= StateHasChanged;
            ProjectApiServices!.RefreshGrid -= RefreshGrid;
        }
        #endregion

        #region SECTIONS METHODS
        #region METHODS OPEN DIALOG TO EDIT/ADD/READ
        private async void OnClickAddSectionsAsync()
        {
            if(ProjectData!.SubdivisionId == null || ProjectData!.SubdivisionId == 0)
            {
                NotifyAcces(string.Empty, "Debes de seleccionar un fraccionamiento antes de ingresar una sección", NotificationSeverity.Warning);
                return;
            }

            var newSection = new ProjectSectionDataDto() { ProjectId = GetProjectId()};
            await OpenSectionsDialogAsync(newSection, TipoEstadoControl.Alta, "Agregar nueva Sección");
        }

        private async void OnClickEditSection(ProjectSectionDataDto section)
        {
            if(section == null) return;
            await OpenSectionsDialogAsync(section!, TipoEstadoControl.Edicion, $"Editar Sección [{section?.Name ?? "-"}]");
        }

        private async void OnDataGridRowDoubleClickSection(DataGridRowMouseEventArgs<ProjectSectionDataDto>  section)
        {
            if(section.Data == null) return;
            await OpenSectionsDialogAsync(section.Data, TipoEstadoControl.Lectura, $"Sección [{section.Data.Name}]");
        }

        private async Task OpenSectionsDialogAsync(ProjectSectionDataDto objectSection, TipoEstadoControl state, string title)
        {
            try
            {
                var result = await Dialog!.OpenAsync<ProjectDetSections>(title: title,
                    parameters: new Dictionary<string, object>() {
                        { "ActionForm", state},
                        { "SectionData", objectSection },
                        { "IsDialogOrigen", true },
                        { "SubdivisionId", ProjectData!.SubdivisionId ?? 0 }
                    },
                    options: new DialogOptions()
                    {
                        CloseDialogOnOverlayClick = true,
                        AutoFocusFirstElement = true,
                        CloseDialogOnEsc = true,
                        Width = "700px"
                    }
                );

                if (result != null && result is ProjectSectionDataDto)
                {
                    var section = (ProjectSectionDataDto?)result;
                    if (state == TipoEstadoControl.Alta)
                    {
                        if(GetSectionList().FirstOrDefault(item => item.Name == section!.Name) != null)
                        {
                            NotifyAcces(string.Empty, "No puede haber dos secciones con el mismo nombre", NotificationSeverity.Error);
                            return;
                        }

                        section!.Sequence = GetSectionList().Where(item => item.Active != false).ToList()!.Count + 1;

                        GetSectionList().Add(section);
                        UpdateSequence<ProjectSectionDataDto>(GetSectionList());
                    }
                    else
                    {
                        int index = GetSectionList().IndexOf(objectSection);
                        GetSectionList()[index] = section!;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            SectionsGrid?.Reload();
            StateHasChanged();
        }
        #endregion

        #region METHODS MOVE
        private void OnClickMoveUpSections(ProjectSectionDataDto section)
        {
            var newList = MoveItemUp<ProjectSectionDataDto>(section, GetSectionList());
            UpdateSectionsList(newList);
        }

        private void OnClickMoveDownSections(ProjectSectionDataDto section)
        {
            if (section == null) return;

            var newList = MoveItemDown<ProjectSectionDataDto>(section, GetSectionList());
            UpdateSectionsList(newList);
        }
        #endregion

        #region METHODS DELETE
        private void OnClickDeleteSection(ProjectSectionDataDto section)
        {
            GetSectionList().Remove(section);
            UpdateSequence<ProjectSectionDataDto>(GetSectionList());
            RefreshGrid();
        }
        #endregion

        #region UTILS
        private bool IsSectionSelected() => SectionsSelected?.Any() ?? false;

        private Guid? GetSelectedSectionGuid() => SectionsSelected.FirstOrDefault()?.SectionGuid;
        
        private List<ProjectSectionDataDto> GetSectionList() => ProjectData?.ProjectSectionData ?? [];

        private void UpdateSectionsList(List<ProjectSectionDataDto> updatedList)
        {
            GetSectionList().Clear();
            GetSectionList().AddRange(updatedList);
            SectionsGrid!.Reload();
        }
        #endregion
        #endregion

        #region PHASES METHODS
        #region METHODS OPEN DIALOG TO EDIT/ADD/READ
        private async void OnClickAddPhaseAsync()
        {
            Guid? sectionGuid = GetSelectedSectionGuid();
            var newPhase = new ProjectSectionPhaseDto()
            {
                SectionGuidTemp = sectionGuid,
                ProjectId = GetProjectId()
            };

            await OpenPhasesDialogAsync(newPhase, TipoEstadoControl.Alta, $"Agregar nueva Fase");
        }

        private async void OnClickEditPhase(ProjectSectionPhaseDto phase) => await OpenPhasesDialogAsync(phase, TipoEstadoControl.Edicion, $"Editar Fase [{phase.Name}]");

        private async void OnDataGridRowDoubleClickPhase(DataGridRowMouseEventArgs<ProjectSectionPhaseDto> phase)
        {
            if (phase.Data == null) return;
            await OpenPhasesDialogAsync(phase.Data, TipoEstadoControl.Lectura, $"Fase [{phase.Data.Name}]");
        }

        private async Task OpenPhasesDialogAsync(ProjectSectionPhaseDto objectPhases, TipoEstadoControl state, string title)
        {
            try
            {
                var result = await Dialog!.OpenAsync<ProjectDetPhase>(title: title,
                    parameters: new Dictionary<string, object>() {
                        { "PhaseData",  objectPhases },
                        { "IsDialogOrigen", true },
                        { "ActionForm", state }
                    },
                    options: new DialogOptions()
                    {
                        CloseDialogOnOverlayClick = true,
                        AutoFocusFirstElement = true,
                        CloseDialogOnEsc = true,
                        Width = "700px"
                    }
                );

                if (result != null && result is ProjectSectionPhaseDto)
                {
                    var phase = (ProjectSectionPhaseDto?)result;
                    if (state == TipoEstadoControl.Alta)
                    {
                        phase!.Sequence = GetActualSectionPhasesList()!.Count + 1;
                        GetActualSectionPhasesList().Add(phase!);
                    }
                    else
                    {
                        int index = GetActualSectionPhasesList().IndexOf(objectPhases);
                        GetActualSectionPhasesList()[index] = phase!;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            PhasesGrid?.Reload();
            StateHasChanged();
        }
        #endregion

        #region METHODS DELETE
        private void OnClickDeletePhase(ProjectSectionPhaseDto phase) 
        {
            GetActualSectionPhasesList().Remove(phase);
            UpdateSequence<ProjectSectionPhaseDto>(GetActualSectionPhasesList());
            
            RefreshGrid();
        }
        #endregion

        #region METHODS MOVE
        private void OnClickMoveUpPhase(ProjectSectionPhaseDto phase)
        {
            var newList = MoveItemUp<ProjectSectionPhaseDto>(phase, GetActualSectionPhasesList());
            UpdatePhasesList(newList);
        }

        private void OnClickMoveDonwPhase(ProjectSectionPhaseDto phase)
        {
            var newList = MoveItemDown<ProjectSectionPhaseDto>(phase, GetActualSectionPhasesList());
            UpdatePhasesList(newList);
        }
        #endregion

        #region UTILS
        /// <summary>
        /// Get list of phases by Section Selected
        /// </summary>
        /// <returns>List<ProjectSectionPhaseDto> </returns>
        private List<ProjectSectionPhaseDto> GetActualSectionPhasesList() => GetSectionList().FirstOrDefault(item => item.SectionGuid == GetSelectedSectionGuid() && item.Active == true)?.ProjectSectionPhase ?? [];

        private void UpdatePhasesList(List<ProjectSectionPhaseDto> updatedPhases)
        {
            GetActualSectionPhasesList().Clear();
            GetActualSectionPhasesList().AddRange(updatedPhases);
            PhasesGrid?.Reload();
        }
        #endregion
        #endregion

        #region UTILS METHODS
        private void NotifyAcces(string summary, string details, NotificationSeverity severity) => Notification!.Notify(new() { Severity = severity, Detail = details, Summary = summary });

        private int? GetProjectId() => ProjectData?.ProjectId;

        private bool IsDisabledForm() => StateForm == TipoEstadoControl.Lectura;

        private void RefreshGrid()
        {
            SectionsGrid?.Reload();
            PhasesGrid?.Reload();
            StateHasChanged();
        }

        #region RESIZE 
        private FlexWrap GetFlexWrap() => BreakpointService!.GetCurrentBreakpoint() == Breakpoint.Xs ? FlexWrap.Wrap : FlexWrap.NoWrap;

        private string GetWithOfColumOptions() => BreakpointService!.GetCurrentBreakpoint() switch
        {
            Breakpoint.Xs => "70",
            Breakpoint.Sm => "75",
            Breakpoint.Md => "80",
            _ => "65"
        }; 
        #endregion
        #endregion

        #region MOVE UP/DOWN LIST
        private List<T> MoveItemUp<T>(T item, List<T> listItem) where T : class
        {
            if(listItem.Count <= 1) return listItem;

            int index = listItem.IndexOf(item);

            // Verificamos si no es el primer elemento
            if (index == 0) return listItem;

            // Intercambiamos las posiciones con el anterior
            var temp = listItem[index - 1];
            listItem[index - 1] = listItem[index];
            listItem[index] = temp;

            return UpdateSequence(listItem);
        }

        private List<T> MoveItemDown<T>(T item, List<T> listItem) where T : class
        {
            if (listItem.Count <= 0) return [];

            int index = listItem.IndexOf(item);

            // Verificamos si no es el último elemento
            if (index == listItem.Count - 1) return listItem;

            // Intercambiamos las posiciones con el siguiente
            var temp = listItem[index + 1];
            listItem[index + 1] = listItem[index];
            listItem[index] = temp;

            return UpdateSequence(listItem);
        }

        private List<T> UpdateSequence<T>(List<T> listItem) where T : class
        {
            int sequence = 0;
            foreach (var item in listItem)
            {
                // Asegurémonos de que la propiedad Sequence está presente y que no sea null
                var sequenceProperty = item.GetType().GetProperty("Sequence");

                if (sequenceProperty != null && sequenceProperty.CanWrite)
                {
                    // Si el valor de Sequence es null, no lo modificamos
                    var currentValue = sequenceProperty.GetValue(item);
                    if (currentValue != null)
                    {
                        sequenceProperty.SetValue(item, sequence + 1);
                        sequence++;
                    }
                }
            }

            return listItem.OrderBy(x => // Usamos reflexión para obtener "Sequence" y asegurarnos de que sea null o no
            {
                var sequenceValue = x.GetType().GetProperty("Sequence")?.GetValue(x);
                return sequenceValue == null ? 1 : 0;
            })
            .ThenBy(x => // Ordenamos por el valor de "Sequence"
            {
                var sequenceValue = x.GetType().GetProperty("Sequence")?.GetValue(x);
                return sequenceValue;
            })
            .ToList();
        }

        #endregion

        public void OpenTab()
        {
            // Crear instancia TabNubetico
            TabNubetico tabNubetico = new()
            {
                EstadoControl = TipoEstadoControl.Alta,
                Icono = this.IconoBase, // fa-circle-info
                Text = "Secciones Proyecto",
                TipoControl = typeof(ProjectSectionDetailsPage),
                Repetir = true
            };

            // Instanciar componente contenido en TabNubetico
            tabNubetico.Componente = builder =>
            {
                builder.OpenComponent(0, tabNubetico.TipoControl);
                builder.AddComponentReferenceCapture(1, instance =>
                {
                    // Asegurarnos que el componente interno instanciado hereda el componente base
                    if (instance is NbBaseComponent nbComponent)
                    {
                        tabNubetico.InstanciaComponente = nbComponent;
                        // Establecer el menú inicial para el componente
                        nbComponent.EstadoControl = TipoEstadoControl.Alta;
                        nbComponent.TriggerMenuUpdate();
                    }

                });
                builder.CloseComponent();
            };

            this.AgregarTabNubetico(tabNubetico);
        }
    }
}
