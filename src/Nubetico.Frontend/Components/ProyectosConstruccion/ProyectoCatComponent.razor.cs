using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Nubetico.Frontend.Components.Core.Shared;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Radzen;
using Radzen.Blazor;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Frontend.Pages.ProyectosConstruccion;
using Nubetico.Frontend.Services.Core;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class ProyectoCatComponent : NbBaseComponent, IDisposable
    {
        #region INEJECTIONS
        [Inject] private IStringLocalizer<SharedResources>? LocalizerServices { get; set; }
        [Inject] private ProjectServices? ProjectApiServices { get; set; }
        [Inject] private NotificationService? NotificationService { get; set; }
        [Inject] private GlobalBreakpointService? BreakpointService { get; set; }
        #endregion

        #region ALL PROPERTYS
        #region GRID PROPERTYS
        private RadzenDataGrid<ProyectsGridDto>? GridCatProyectos { get; set; }
        private bool IsLoading { get; set; } = false;
        private IEnumerable<ProyectsGridDto>? ProyectsList { get; set; } = [];
        public IList<ProyectsGridDto> SelectedProyect { get; set; } = [];
        private ProjectsPaginatedRequestDto RequestForm { get; set; } = new();
        private int Count { get; set; }
        #endregion

        #region Form PROPERTYS
        private IEnumerable<ElementsDropdownForm> Branch { get; set; } = [];
        private IEnumerable<ElementsDropdownForm> Status { get; set; } = [];
        private IEnumerable<ElementsDropdownForm> Subdivision { get; set; } = [];
        #endregion
        #endregion

        #region ALL METHODS
        #region METHODS LIFE CILCE BLAZOR
        protected override void OnInitialized()
        {
            ProjectApiServices!.RefreshProjectCat += RefreshGridByAction;
            ProjectApiServices!.IsProjectCatIsOpen = true;
            BreakpointService!.OnChange += StateHasChanged;
            TriggerMenuUpdate();
        }
        protected override async Task OnInitializedAsync()
        {
            await LoadFormDataAsync();

            await OnClickFilterProjectsAsync();

            await base.OnInitializedAsync();
        }

        public void Dispose()
        {
            ProjectApiServices!.RefreshProjectCat -= RefreshGridByAction;
            ProjectApiServices!.IsProjectCatIsOpen = false;
            BreakpointService!.OnChange -= StateHasChanged;
        }
        #endregion

        #region DATAGRID
        private async void RefreshGridByAction()
        {
            await OnClickFilterProjectsAsync();
            GridCatProyectos?.Reload();
        }

        private async Task OnClickFilterProjectsAsync()
        {
            var args = new LoadDataArgs { Top = 20, Skip = 0 };
            await LoadData(args);
        }

        private async Task LoadData(LoadDataArgs args)
        {
            if(IsLoading) return;

            IsLoading = true;

            RequestForm.OrderBy = args.Sorts != null ? string.Join(",", args.Sorts.Select(s => $"{s.Property} {(s.SortOrder == SortOrder.Descending ? "desc" : "asc")}")) : "";
            RequestForm.Limit = args.Top ?? 20;
            RequestForm.OffSet = args.Skip ?? 0;

            var result = await ProjectApiServices!.GetPaginatedProjectsAsync(request: RequestForm);
            if (result == null || result.StatusCode > 300 || !result.Success)
            {
                ProyectsList = [];
            }
            else
            {
                var data = result.Data;
                Count = data!.RecordsFiltered;
                ProyectsList = data!.Data;
            }
            
            IsLoading = false;
        }

        private async Task OnDataGridRowDoubleClick(DataGridRowMouseEventArgs<ProyectsGridDto> args) => await OnClickOpenAsync(TipoEstadoControl.Lectura);

        #endregion

        #region INIT FORM DATA
        private async Task LoadFormDataAsync()
        {
            IsLoading = true;

            var result = await ProjectApiServices!.GetProjectFormDataAsync();
            if (!(result == null || result.StatusCode > 300 || !result.Success || result.Data is not ProjectFormDataDto))
            {
                try
                {
                    var formData = result.Data!;

                    Subdivision = formData.Subdivision;
                    Status = formData.Status;
                    Branch = formData.Branch;
                }
                catch (Exception ex) {  }
            }
            
            IsLoading = false;
        }
        #endregion

        #region BUTTON LIST
        protected override List<RadzenMenuItem> GetMenuItems()
        {
            var menusDefinidos = MenuItemsFactory.GetBaseMenuItems(LocalizerServices!);
            var menusMostrar = new List<RadzenMenuItem>();

            void AgregarMenuSiExiste(string comando, EventCallback<MenuItemEventArgs> onClick)
            {
                var menu = menusDefinidos.FirstOrDefault(m => m.Attributes != null
                    && m.Attributes.TryGetValue("comando", out var comandoValue)
                    && comandoValue.ToString() == comando);

                if (menu != null)
                {
                    menu!.Click = onClick;
                    menusMostrar.Add(menu);
                }
            }

            AgregarMenuSiExiste(BaseMenuCommands.OPEN, EventCallback.Factory.Create<MenuItemEventArgs>(this, async () => await OnClickOpenAsync(TipoEstadoControl.Lectura)));
            AgregarMenuSiExiste(BaseMenuCommands.ADD, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickAdd));
            AgregarMenuSiExiste(BaseMenuCommands.EDIT, EventCallback.Factory.Create<MenuItemEventArgs>(this, async () => await OnClickOpenAsync(TipoEstadoControl.Edicion)));
            AgregarMenuSiExiste(BaseMenuCommands.REFRESH, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickFilterProjectsAsync));
            AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));

            return menusMostrar;
        }

        private void OnClickAdd() => OpenTab(action: TipoEstadoControl.Alta, name: $"{LocalizerServices!["Shared.Textos.New"]} {LocalizerServices["Shared.Textos.Project"]}", data: new ProjectDataDto());

        private async Task OnClickOpenAsync(TipoEstadoControl typeState)
        {
            var project = SelectedProyect.FirstOrDefault();
            if (project == null)
            {
                NotifyAcces(summary: "Ocurrio un problema", details: $"Debes seleccionar una proyecto para poder {(typeState == TipoEstadoControl.Edicion ? "editarlo" : "abrirlo")}", severity: NotificationSeverity.Error);
                return;
            }

            var result = await ProjectApiServices!.GetFilterProjectAsync(project!.ProjectGuid!.Value);
            if (result == null || !result.Success)
            {
                NotifyAcces(summary: "Ocurrio un problema", details: $"No fue posible recuperar los datos del proyecto [{project.Folio}]", severity: NotificationSeverity.Error);
                return;
            }

            OpenTab(action: typeState, name: $"{LocalizerServices!["Shared.Textos.Project"]} [{project.Folio}]", data: result!.Data!);
        }

        private void OnClickClose() => this.CerrarTabNubetico();

        public void OpenTab(TipoEstadoControl action, string name, ProjectDataDto data)
        {
            // Crear instancia TabNubetico
            TabNubetico tabNubetico = new()
            {
                EstadoControl = action,
                Icono = GetIconByControlState(action),
                Text = name,
                TipoControl = typeof(ProyectosDetPage),
                Repetir = true
            };

            // Instanciar componente contenido en TabNubetico
            tabNubetico.Componente = builder =>
            {
                builder.OpenComponent(0, tabNubetico.TipoControl);
                builder.AddAttribute(1, "ProjectData", data);
                builder.AddComponentReferenceCapture(1, instance =>
                {
                    // Asegurarnos que el componente interno instanciado hereda el componente base
                    if (instance is NbBaseComponent nbComponent)
                    {
                        tabNubetico.InstanciaComponente = nbComponent;
                        // Establecer el menú inicial para el componente
                        nbComponent.IconoBase = GetIconByControlState(TipoEstadoControl.Lectura);
                        nbComponent.EstadoControl = action;
                        nbComponent.TriggerMenuUpdate();
                    }

                });
                builder.CloseComponent();
            };

            this.AgregarTabNubetico(tabNubetico);
        }

        private static string GetIconByControlState(TipoEstadoControl typeState) => typeState switch
        {
            TipoEstadoControl.Edicion => $"{MenuItemsFactory.MenuIconDictionary["editar"]}",
            TipoEstadoControl.Alta => $"{MenuItemsFactory.MenuIconDictionary["agregar"]}",
            _ => "e58a"
        };

        #endregion

        #region UTILS
        private void NotifyAcces(string summary, string details, NotificationSeverity severity) => NotificationService!.Notify(new()
        {
            Severity = severity,
            Summary = summary,
            Detail = details
        });
        #endregion
        #endregion
    }
}
