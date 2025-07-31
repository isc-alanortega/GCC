using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Contratistas;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class ContratistaCatComponent : NbBaseComponent
    {
        [Inject]
        protected IStringLocalizer<SharedResources> Localizer { get; set; }
        [Inject] protected ContratistaService ContratistasService { get; set; }
        private RadzenDataGrid<ContratistaGridResultSet> GridContratistas { get; set; }
        private List<ContratistaGridResultSet> ListContratistas { get; set; } = new();
        private int Count { get; set; }
        private bool IsLoading { get; set; } = false;
        private int RowsPerPage { get; set; } = 10;
        public IList<ContratistaGridResultSet> ContratistasSeleccionados { get; set; } = [];
        private FiltroContratistasNubeticoGridDto Filtro { get; set; } = new FiltroContratistasNubeticoGridDto();
        private List<BasicItemSelectDto> SelectEstadosContratista = new List<BasicItemSelectDto>();
        public bool busy { get; set; } = false;

        

        protected override async Task OnInitializedAsync()
        {
            TriggerMenuUpdate();

            await RefreshGridAsync("NombreComercial ASC", RowsPerPage, 0);
        }

        protected override List<RadzenMenuItem> GetMenuItems()
        {
            var baseMenu = MenuItemsFactory.GetBaseMenuItems(Localizer);
            var displayedMenu = new List<RadzenMenuItem>();

            void AddMenu(string comando, EventCallback<MenuItemEventArgs> onClick)
            {
                var menu = baseMenu.FirstOrDefault(m => m.Attributes != null
                    && m.Attributes.TryGetValue("comando", out var comandoValue)
                    && comandoValue.ToString() == comando);

                if (menu != null)
                {
                    menu.Click = onClick;
                    displayedMenu.Add(menu);
                }
            }

            AddMenu(BaseMenuCommands.OPEN, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnAbrirClick));
            AddMenu(BaseMenuCommands.ADD, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnAgregarClick));
            AddMenu(BaseMenuCommands.EDIT, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnEditarClick));
            AddMenu(BaseMenuCommands.REFRESH, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnRefrescarClickAsync));
            AddMenu(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnCerrarClick));

            return displayedMenu;
        }

        #region Eventos del menú

        private void OnAgregarClick(MouseEventArgs args)
        {
            string? guidContratista = null;

            // Crear instancia TabNubetico
            TabNubetico tabNubetico = new TabNubetico
            {
                EstadoControl = TipoEstadoControl.Alta,
                Icono = $"{MenuItemsFactory.MenuIconDictionary["agregar"]}",
                Text = Localizer["Core.ProyectosConstruccion.ContratistasAgregar"],
                TipoControl = typeof(ContratistaDetComponent),
                Repetir = true
            };

            // Instanciar componente contenido en TabNubetico
            tabNubetico.Componente = builder =>
            {
                builder.OpenComponent(0, tabNubetico.TipoControl);
                builder.AddAttribute(1, "GuidContratista", guidContratista);
                builder.AddComponentReferenceCapture(1, instance =>
                {
                    // Asegurarnos que el componente interno instanciado hereda el componente base
                    if (instance is NbBaseComponent nbComponent)
                    {
                        tabNubetico.InstanciaComponente = nbComponent;
                        // Establecer el menú inicial para el componente
                        nbComponent.EstadoControl = TipoEstadoControl.Alta;

                        if (!string.IsNullOrEmpty(this.IconoBase))
                            nbComponent.IconoBase = this.IconoBase;

                        nbComponent.TriggerMenuUpdate();
                    }
                });
                builder.CloseComponent();
            };

            this.AgregarTabNubetico(tabNubetico);
            StateHasChanged();

        }

        private async void OnAbrirClick(MouseEventArgs args)
        {
            if (ContratistasSeleccionados.Count == 0)
                return;

            var contratistaSeleccionado = ContratistasSeleccionados.FirstOrDefault();

            if (contratistaSeleccionado != null)
                AbrirDetalleContratista(contratistaSeleccionado, TipoEstadoControl.Lectura);
        }

        private async void OnEditarClick(MouseEventArgs args)
        {
            if (ContratistasSeleccionados.Count == 0)
                return;

            var contratistaSeleccionado = ContratistasSeleccionados.FirstOrDefault();

            if (contratistaSeleccionado != null)
                AbrirDetalleContratista(contratistaSeleccionado, TipoEstadoControl.Edicion);
        }

        private async Task OnRefrescarClickAsync(MouseEventArgs args)
        {
            await RefreshGridAsync("NombreComercial ASC", this.RowsPerPage, 0);
        }

        private void OnCerrarClick(MouseEventArgs args)
        {
            this.CerrarTabNubetico();
        }

        #endregion

        #region funciones

        private async Task AbrirDetalleContratista(ContratistaGridResultSet contratistaSeleccionado, TipoEstadoControl estadoControl)
        {
            string? guidContratista = contratistaSeleccionado.UUID.ToString();
            if (contratistaSeleccionado == null)
            {
                Console.WriteLine("No se seleccionó ningún contratista.");
                return;
            }

            var result = await ContratistasService.GetContratistaByIdAsync(contratistaSeleccionado.IdContratista);
            if (result == null)
            {
                Console.WriteLine("No se pudo obtener los detalles del contratista.");
                return;
            }
            // Crear instancia TabNubetico
            TabNubetico tabNubetico = new TabNubetico
            {
                EstadoControl = estadoControl,
                Icono = estadoControl == TipoEstadoControl.Edicion
                            ? MenuItemsFactory.MenuIconDictionary["editar"]
                            : this.IconoBase,

                Text = $"{Localizer["Core.ProyectosConstruccion.Contratista"]} [{contratistaSeleccionado.NombreComercial}]",
                TipoControl = typeof(ContratistaDetComponent),
                Repetir = true
            };

            // Instanciar componente contenido en TabNubetico
            tabNubetico.Componente = builder =>
            {
                builder.OpenComponent(0, tabNubetico.TipoControl);
                builder.AddAttribute(1, "GuidContratista", guidContratista);
                builder.AddAttribute(2, "ContratistaData", result);
                builder.AddComponentReferenceCapture(1, instance =>
                {
                    // Asegurarnos que el componente interno instanciado hereda el componente base
                    if (instance is NbBaseComponent nbComponent)
                    {
                        tabNubetico.InstanciaComponente = nbComponent;
                        // Establecer el estado inicial para el componente
                        nbComponent.EstadoControl = estadoControl;

                        if (!string.IsNullOrEmpty(this.IconoBase))
                            nbComponent.IconoBase = this.IconoBase;

                        nbComponent.TriggerMenuUpdate();
                    }
                });
                builder.CloseComponent();
            };

            this.AgregarTabNubetico(tabNubetico);
        }

        private async Task DataGridRowDoubleClick(DataGridRowMouseEventArgs<ContratistaGridResultSet> args)
        {
            if (args.Data != null)
                AbrirDetalleContratista(args.Data, TipoEstadoControl.Lectura);
        }

        private async Task LoadDataAsync(LoadDataArgs args)
        {
            //string orderBy = string.Join(",", args.Sorts.Select(s => $"{s.Property} {(s.SortOrder == SortOrder.Descending ? "desc" : "asc")}"));
            //await RefreshGridAsync(orderBy, args.Top ?? 0, args.Skip ?? 0);
        }

        private async Task RefreshGridAsync(string orderBy, int top, int skip)
        {
            IsLoading = true;

            var result = await ContratistasService.GetContratistaPaginadoAsync(top, skip, orderBy, Filtro.Nombre, Filtro.RFC);

            if (!result.Success || result.Data == null)
            {
                IsLoading = false;
                ListContratistas.Clear();
                Count = 0;
                return;
            }


                ListContratistas = result.Data.Data;
                Count = result.Data.RecordsTotal;

            IsLoading = false;
        }

        #endregion

        public class FiltroContratistasNubeticoGridDto
        {
            public string RFC { get; set; } = "";
            public string Nombre { get; set; } = "";
        }
    }
}
