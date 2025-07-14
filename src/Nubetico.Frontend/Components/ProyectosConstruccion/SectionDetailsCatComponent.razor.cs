using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Nubetico.Frontend.Components.Core.Shared;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Pages.ProyectosConstruccion;
using Nubetico.Frontend.Services.Core;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.ProjectSectionDetails;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Radzen;
using Radzen.Blazor;
using Radzen.Blazor.Rendering;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class SectionDetailsCatComponent : NbBaseComponent, IDisposable
    {
        #region INJECTIONS
        [Inject] private GlobalBreakpointService? BreakpointService { get; set; }
        [Inject] private IStringLocalizer<SharedResources> Localizer { get; set; }
        [Inject] private SectionDetailsServices? SectionApiServices { get; set; }
        [Inject] private NotificationService? Notification { get; set; }
        #endregion

        #region GRID PROPERTYS
        private RadzenDataGrid<SectionDetailsDto>? SectionGrid { get; set; }
        private IEnumerable<SectionDetailsDto>? SectionData { get; set; } = [];
        public IList<SectionDetailsDto> SectionSelected { get; set; } = [];
        private int CountGrid { get; set; }
        private LoadDataArgs Arguments { get; set; } = new() { Top = 20, Skip = 0 };
        #endregion

        #region PROPERTYS
        private bool IsLoading { get; set; } = false;
        private RequestSectionDetailsCatDto Request { get; set; } = new();
        #endregion

        #region LIFE CILCE BLAZOR
        protected override void OnInitialized()
        {
            BreakpointService!.OnChange += StateHasChanged;
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadData(Arguments);

            await base.OnInitializedAsync();
        }

        public void Dispose()
        {
            BreakpointService!.OnChange -= StateHasChanged;
        }
        #endregion

        #region METHODS GRID
        private async void DataGridRowDoubleClick(DataGridRowMouseEventArgs<SectionDetailsDto> args)
        {

        }


        private async Task LoadData(LoadDataArgs args)
        {
            if(IsLoading) return;

            IsLoading = true;
            
            Request.OrderBy = args.Sorts != null ? string.Join(",", args.Sorts.Select(s => $"{s.Property} {(s.SortOrder == SortOrder.Descending ? "desc" : "asc")}")) : "";
            Request.Limit = args.Top ?? 20;
            Request.OffSet = args.Skip ?? 0;

            var result = await SectionApiServices!.GetSectionsPaginatedListAsync(Request);
            if(result == null || result.StatusCode > 300 || result.Data == null)
            {

                return;
            }

            CountGrid = result.Data.RecordsTotal;
            SectionData = result.Data.Data;

            IsLoading = false;
        }
        #endregion

        #region FORM

        private async Task OnClickFilterSectionsAsync()
        {
            await LoadData(Arguments);
        }
        #endregion

        #region BUTTON LIST
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

            AgregarMenuSiExiste(BaseMenuCommands.OPEN, EventCallback.Factory.Create<MenuItemEventArgs>(this, async () => await OnClickOpenAsync(TipoEstadoControl.Lectura)));
            //AgregarMenuSiExiste(BaseMenuCommands.ADD, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickAdd));
            AgregarMenuSiExiste(BaseMenuCommands.EDIT, EventCallback.Factory.Create<MenuItemEventArgs>(this, async () => await OnClickOpenAsync(TipoEstadoControl.Edicion)));
            AgregarMenuSiExiste(BaseMenuCommands.REFRESH, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickFilterSectionsAsync));
            AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));

            return menusMostrar;
        }

        //private void OnClickAdd() => OpenTab(action: TipoEstadoControl.Alta, name: $"{Localizer!["Shared.Textos.New"]} {Localizer["Shared.Textos.Project"]}", data: new ResponseSectionDetailsDto() { SectionDetails = new(), SectionLots = [] });

        private async Task OnClickOpenAsync(TipoEstadoControl typeState)
        {
            var section = SectionSelected.FirstOrDefault();
            if (section == null)
            {
                NotifyAcces(summary: "Ocurrio un problema", details: $"Debes seleccionar una proyecto para poder {(typeState == TipoEstadoControl.Edicion ? "editarlo" : "abrirlo")}", severity: NotificationSeverity.Error);
                return;
            }

            var result = await SectionApiServices!.GetSectionFilterByIdAsync(section!.SectionGuid!);
            if (result == null || !result.Success)
            {
                NotifyAcces(summary: "Ocurrio un problema", details: $"No fue posible recuperar los datos del proyecto [{section.Section}]", severity: NotificationSeverity.Error);
                return;
            }

            OpenTab(action: typeState, name: $"{Localizer!["Shared.Textos.Project"]} [{section.Section}]", data: result!.Data!);
        }

        private void OnClickClose() => this.CerrarTabNubetico();

        public void OpenTab(TipoEstadoControl action, string name, ResponseSectionDetailsDto data)
        {
            // Crear instancia TabNubetico
            TabNubetico tabNubetico = new()
            {
                EstadoControl = action,
                Icono = GetIconByControlState(action),
                Text = name,
                TipoControl = typeof(ProjectSectionDetailsPage),
                Repetir = true
            };

            // Instanciar componente contenido en TabNubetico
            tabNubetico.Componente = builder =>
            {
                builder.OpenComponent(0, tabNubetico.TipoControl);
                builder.AddAttribute(1, "SectionDetails", data.SectionDetails);
                builder.AddAttribute(2, "SectionLots", data.SectionLots);
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
            _ => "e443" // fa-puzzle
        };
        #endregion

        #region UTILS
        private void NotifyAcces(string summary, string details, NotificationSeverity severity) => Notification!.Notify(new()
        {
            Severity = severity,
            Summary = summary,
            Detail = details
        });
        #endregion
    }
}
