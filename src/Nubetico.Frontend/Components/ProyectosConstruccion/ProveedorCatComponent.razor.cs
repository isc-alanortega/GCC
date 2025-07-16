using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Nubetico.Frontend.Components.Core.Shared;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Models;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class ProveedorCatComponent : NbBaseComponent
    {
        [Inject]
        protected IStringLocalizer<SharedResources> Localizer { get; set; }
        private RadzenDataGrid<ProveedorGridResultSet>? GridProveedores { get; set; }
        private List<ProveedoresDto>? ListaProveedores { get; set; }
        private int Count { get; set; }
        private bool IsLoading { get; set; } = false;
        private int RowsPerPage { get; set; } = 10;
        //public IList<ProveedoresDto> ProveedoresSeleccionados { get; set; } = new List<ProveedoresDto>();
        public IList<ProveedorGridResultSet> ProveedoresSeleccionados { get; set; } = [];
        //private IList<InsumosDto> SelectedSupply { get; set; } = [];
        private FiltroProveedoresNubeticoGridDto Filtro { get; set; } = new FiltroProveedoresNubeticoGridDto();
        private List<BasicItemSelectDto> SelectEstadosProveedor = new List<BasicItemSelectDto>();

        private List<ProveedorGridResultSet> proveedores = new();

        [Inject] public ProveedorServices ProveedoresService { get; set; }
        //public bool busy { get; set; } = false;



        protected override async Task OnInitializedAsync()
        {
            TriggerMenuUpdate();

            //ListaProveedores = new List<ProveedoresDto> { p1, p2, p3 };
            await LoadProveedores();


        }
        private async Task LoadProveedores()
        {
            try
            {
                proveedores = await ProveedoresService.GetAllProveedoresAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar proveedores: {ex.Message}");
            }
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
            string? guidProveedor = null;

            // Crear instancia TabNubetico
            TabNubetico tabNubetico = new TabNubetico
            {
                EstadoControl = TipoEstadoControl.Alta,
                Icono = $"{MenuItemsFactory.MenuIconDictionary["agregar"]}",
                Text = Localizer["Core.ProyectosConstruccion.ProveedoresAgregar"],
                TipoControl = typeof(ProveedorDetComponent),
                Repetir = true
            };

            // Instanciar componente contenido en TabNubetico
            tabNubetico.Componente = builder =>
            {
                builder.OpenComponent(0, tabNubetico.TipoControl);
                builder.AddAttribute(1, "GuidProveedor", guidProveedor);
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

        private void OnAbrirClick(MouseEventArgs args)
        {
            if (ProveedoresSeleccionados.Count == 0)
                return;

            var proveedorSeleccionado = ProveedoresSeleccionados.FirstOrDefault();

            if (proveedorSeleccionado != null)
                AbrirDetalleProveedor(proveedorSeleccionado, TipoEstadoControl.Lectura);
        }

        private void OnEditarClick(MouseEventArgs args)
        {
            if (ProveedoresSeleccionados.Count == 0)
                return;

            var proveedorSeleccionado = ProveedoresSeleccionados.FirstOrDefault();

            if (proveedorSeleccionado != null)
                AbrirDetalleProveedor(proveedorSeleccionado, TipoEstadoControl.Edicion);
        }

        private async Task OnRefrescarClickAsync(MouseEventArgs args)
        {
            await RefreshGridAsync("NombreCompleto asc", this.RowsPerPage, 0);
        }

        private void OnCerrarClick(MouseEventArgs args)
        {
            this.CerrarTabNubetico();
        }

        #endregion

        #region funciones

        private async Task AbrirDetalleProveedor(ProveedorGridResultSet proveedorSeleccionado, TipoEstadoControl estadoControl)
        {
            string? guidUsuario = proveedorSeleccionado.UUID.ToString();
            if (proveedorSeleccionado == null)
            {
                Console.WriteLine("No se seleccionó ningún proveedor.");
                return;
            }

            var result = await ProveedoresService.GetProveedorByIdAsync(proveedorSeleccionado.IdProveedor);
            if (result == null)
            {
                Console.WriteLine("No se pudo obtener los detalles del proveedor.");
                return;
            }
            // Crear instancia TabNubetico
            TabNubetico tabNubetico = new TabNubetico
            {
                EstadoControl = estadoControl,
                Icono = estadoControl == TipoEstadoControl.Edicion
                            ? $"{MenuItemsFactory.MenuIconDictionary["editar"]}"
                            : this.IconoBase,

                Text = $"{Localizer["Core.ProyectosConstruccion.Proveedor"]} [{proveedorSeleccionado.NombreComercial}]",
                TipoControl = typeof(ProveedorDetComponent),
                Repetir = true
            };

            // Instanciar componente contenido en TabNubetico
            tabNubetico.Componente = builder =>
            {
                builder.OpenComponent(0, tabNubetico.TipoControl);
                builder.AddAttribute(1, "GuidProveedor", guidUsuario);
                builder.AddAttribute(2, "ProveedorData", result);
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

        private async Task DataGridRowDoubleClick(DataGridRowMouseEventArgs<ProveedorGridResultSet> args)
        {
            if (args.Data != null)
                AbrirDetalleProveedor(args.Data, TipoEstadoControl.Lectura);
        }

        private async Task LoadDataAsync(LoadDataArgs args)
        {
            //string orderBy = string.Join(",", args.Sorts.Select(s => $"{s.Property} {(s.SortOrder == SortOrder.Descending ? "desc" : "asc")}"));
            //await RefreshGridAsync(orderBy, args.Top ?? 0, args.Skip ?? 0);
        }

        private async Task RefreshGridAsync(string orderBy, int top, int skip)
        {
            IsLoading = true;

            //var result = await UsuariosService.GetUsuariosPaginado(top, skip, orderBy, Filtro.Username, Filtro.Nombre, Filtro.IdEstadoUsuario);

            //if (!result.Success || result.Data == null)
            //{
            //    IsLoading = false;
            //    ListaUsuarios.Clear();
            //    Count = 0;
            //    return;
            //}

            //PaginatedListDto<UsuarioNubeticoGridDto>? listaPaginada = JsonConvert.DeserializeObject<PaginatedListDto<UsuarioNubeticoGridDto>>(result.Data.ToString());

            //if (listaPaginada != null)
            //{
            //    ListaUsuarios = listaPaginada.Data;
            //    Count = listaPaginada.RecordsTotal;
            //}

            IsLoading = false;
        }
        public void OpenTab(TipoEstadoControl action, string name, ProveedoresDto data)
        {
            var tabNubetico = new TabNubetico
            {
                EstadoControl = action,
                Icono = GetIconByControlState(action),
                Text = name,
                TipoControl = typeof(ProveedorDetComponent),
                Repetir = true
            };

            tabNubetico.Componente = builder =>
            {
                builder.OpenComponent(0, tabNubetico.TipoControl);
                builder.AddAttribute(1, "ProveedorData", data);
                builder.AddComponentReferenceCapture(1, instance =>
                {
                    if (instance is NbBaseComponent nbComponent)
                    {
                        tabNubetico.InstanciaComponente = nbComponent;
                        nbComponent.IconoBase = GetIconByControlState(action);
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
            _ => "f015" // fa-house
        };



        #endregion

        public class FiltroProveedoresNubeticoGridDto
        {
            public string RFC { get; set; } = "";
            public string Nombre { get; set; } = "";
        }
    }
}
