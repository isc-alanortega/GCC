using Microsoft.AspNetCore.Components;
using Nubetico.Frontend.Models.Static.Core;
using Radzen.Blazor;
using Radzen;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Nubetico.Shared.Dto.ProyectosConstruccion.Models;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Pages.ProyectosConstruccion;
using Nubetico.Frontend.Components.Core.Shared;
using System.Reflection.Metadata.Ecma335;
using System.Net.NetworkInformation;
using DocumentFormat.OpenXml.EMMA;
using Nubetico.Frontend.Components.Core.Dialogs;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class ModelCatComponent : NbBaseComponent
    {
        #region INJECTS
        [Inject] private ModelService? ModelServices { get; set; }
        [Inject] private NotificationService? NotifyService { get; set; }
        #endregion

        #region PROPERTYS
        private bool IsLoading { get; set; } = false;
        private bool IsOpening { get; set; } = false;

        #region GRID PROPERTYS
        private RadzenDataGrid<ModelGridDto>? Grid { get; set; }
        public IList<ModelGridDto> ModelSelected { get; set; } = [];
        #endregion

        private List<ModelGridDto> ModelsGrid { get; set; }
        private ModelGridRequestDto RequestGrid { get; set; } = new ModelGridRequestDto();
        private int Count { get; set; } = 0;
        #endregion

        #region VARIABLES
        private LoadingDialogComponent _loadingDialog;
        #endregion

        #region METHODS
        #region LIFECYCLE BLAZOR METHODS
        protected override void OnInitialized()
        {
            this.TriggerMenuUpdate();
            base.OnInitialized();
        }
        #endregion

        #region EVENTS
        private async void OnClickSearch() => await LoadData(new LoadDataArgs());

        private async void OnDataGridRowDoubleClick(DataGridRowMouseEventArgs<ModelGridDto> args) => await HandleOpenModelAsync(TipoEstadoControl.Lectura);

        private async void OnClickOpen() => await HandleOpenModelAsync(TipoEstadoControl.Lectura);

        private void OnClickAdd() => OpenTab(action: TipoEstadoControl.Alta, name: $"{Localizer!["Shared.Textos.New"]} {Localizer["Subdivisions.Text.Model"]}", data: new ModelDto());
        
        private async void OnClickEdit() => await HandleOpenModelAsync(TipoEstadoControl.Lectura);

        private async void OnClickRefresh() => await LoadData(new LoadDataArgs());

        private void OnClickClose() => this.CerrarTabNubetico();
        #endregion

        #region FORM METHODS
        /// <summary>  
        /// Loads data models from the database.  
        /// </summary>  
        private async Task LoadData(LoadDataArgs args)
        {
            if (IsLoading) return;

            IsLoading = true;

            RequestGrid.OrderBy = args.Sorts != null ? string.Join(",", args.Sorts.Select(s => $"{s.Property} {(s.SortOrder == SortOrder.Descending ? "desc" : "asc")}")) : "";
            RequestGrid.Limit = args.Top ?? 20;
            RequestGrid.Offset = args.Skip ?? 0;

            var result = await ModelServices!.GetPaginatedModelsAsync(request: RequestGrid);
            if (result == null || result.StatusCode > 300 || !result.Success || result.Data is null)
            {
                ModelsGrid = [];
                return;
            }
            else
            {
                var data = result.Data;
                Count = data!.RecordsFiltered;
                ModelsGrid = data!.Data;

            }
            
            IsLoading = false;
        }
        
        /// <summary>  
        /// Handles the opening of a model asynchronously.  
        /// </summary>  
        private async Task HandleOpenModelAsync(TipoEstadoControl state)
        {
            var modelSelected = ModelSelected.FirstOrDefault();
            if (modelSelected == null) 
            {
                Notify(summary: Localizer["Shared.Text.ProblemOcurred"], details: state == TipoEstadoControl.Edicion ? Localizer["Model.Text.Error.SelectToEdit"] : Localizer["Model.Text.Error.SelectToOpen"], severity: NotificationSeverity.Error);
                return;
            }

            if(IsOpening) return;

            IsOpening = true;
            _loadingDialog.Show(message: "Abriendo el detalle del modelo");


            var respose = await ModelServices!.GetModelByIdAsync(modelSelected.ModelId);

            IsOpening = false;
            _loadingDialog.Hide();

            if (respose == null || !respose.Success || respose.StatusCode > 300 || respose.Data is null)
            {
                Notify(summary: Localizer["Shared.Text.ProblemOcurred"], details: $"{Localizer["Model.Text.Error.Open"]} [{modelSelected.Folio}]", severity: NotificationSeverity.Error);
                return;
            }

            OpenTab(action: state, name: $"{Localizer!["Subdivisions.Text.Model"]} [{modelSelected.Folio}]", data: respose.Data!);
        }

        /// <summary>  
        /// Opens a new tab with the specified action, name, and data.  
        /// </summary>  
        public void OpenTab(TipoEstadoControl action, string name, ModelDto data)
        {
            // Crear instancia TabNubetico
            TabNubetico tabNubetico = new()
            {
                EstadoControl = action,
                Icono = GetIconByControlState(action),
                Text = name,
                TipoControl = typeof(ModelsDetComponent),
                Repetir = true
            };

            // Instanciar componente contenido en TabNubetico
            tabNubetico.Componente = builder =>
            {
                builder.OpenComponent(0, tabNubetico.TipoControl);
                builder.AddAttribute(1, "ModelData", data);
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

        /// <summary> Displays a notification with the specified summary, details, and severity. </summary>
        private void Notify(string summary, string details, NotificationSeverity severity) => NotifyService!.Notify(new()
        {
            Severity = severity,
            Summary = summary,
            Detail = details
        });
        #endregion

        #region BUTTONS HEADER METHODS
        protected override List<RadzenMenuItem> GetMenuItems()
        {
            var menusDefinidos = MenuItemsFactory.GetBaseMenuItems(Localizer!);
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

            AgregarMenuSiExiste(BaseMenuCommands.OPEN, EventCallback.Factory.Create<MenuItemEventArgs>(this,OnClickOpen));
            AgregarMenuSiExiste(BaseMenuCommands.ADD, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickAdd));
            AgregarMenuSiExiste(BaseMenuCommands.EDIT, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickEdit));
            AgregarMenuSiExiste(BaseMenuCommands.REFRESH, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickRefresh));
            AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));

            return menusMostrar;
        }

        private static string GetIconByControlState(TipoEstadoControl typeState) => typeState switch
        {
            TipoEstadoControl.Edicion => $"{MenuItemsFactory.MenuIconDictionary["editar"]}",
            TipoEstadoControl.Alta => $"{MenuItemsFactory.MenuIconDictionary["agregar"]}",
            _ => "f015" // fa-house
        };
        #endregion
        #endregion
    }
}
