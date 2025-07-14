using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Nubetico.Frontend.Components.Core.Shared;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class ProveedorCatComponent : NbBaseComponent
    {
        [Inject]
        protected IStringLocalizer<SharedResources> Localizer { get; set; }
        private RadzenDataGrid<ProveedoresDto>? GridProveedores { get; set; }
        private List<ProveedoresDto>? ListaProveedores { get; set; }
        private int Count { get; set; }
        private bool IsLoading { get; set; } = false;
        private int RowsPerPage { get; set; } = 10;
		//public IList<ProveedoresDto> ProveedoresSeleccionados { get; set; } = new List<ProveedoresDto>();
		public IList<ProveedoresDto> ProveedoresSeleccionados { get; set; } = [];
		//private IList<InsumosDto> SelectedSupply { get; set; } = [];
		private FiltroProveedoresNubeticoGridDto Filtro { get; set; } = new FiltroProveedoresNubeticoGridDto();
        private List<BasicItemSelectDto> SelectEstadosProveedor = new List<BasicItemSelectDto>();
        //public bool busy { get; set; } = false;


        ProveedoresDto p1 = new ProveedoresDto()
        {
            Folio = "PRV01",
            EstadoProveedor = "Activo",
            IdEstadoProveedor = 1,
            Nombre = "FERRETERIA LUGO",
            RFC = "FER010101001",
            UUID = Guid.NewGuid(),
            CreditoPesos = "50,000",
            CreditoDolares = "2,500",
            RegimenFiscal = "PERSONA MORAL",
            TieneCredito = true,
            DiasCredito = "30",
            DiasGracia = "5",
            SaldoPesos ="15,000",
            SaldoDolares = "750",
            Web = "www.ferretarialugo.com.mx",
            Correo = "contacto@ferretarialugo.com.mx",
            CuentaContable = "0101010101010101",
            FormaPago = "TRANSFERENCIA"
        };

        ProveedoresDto p2 = new ProveedoresDto()
        {
            Folio = "PRV02",
            EstadoProveedor = "Activo",
            IdEstadoProveedor = 1,
            Nombre = "FERREPACIFICO",
            RFC = "FER020202002",
            UUID = Guid.NewGuid(),
            CreditoPesos = "75,000",
            CreditoDolares = "3,500",
            RegimenFiscal = "PERSONA MORAL",
            TieneCredito = true,
            DiasCredito = "30",
            DiasGracia = "5",
            SaldoPesos = "18,000",
            SaldoDolares = "900",
            Web = "www.ferrepacifico.com",
            Correo = "info@ferretarialugo.com",
            CuentaContable = "0202020202020202",
            FormaPago = "TRANSFERENCIA"
        };

        ProveedoresDto p3 = new ProveedoresDto()
        {
            Folio = "PRV03",
            EstadoProveedor = "Activo",
            IdEstadoProveedor = 1,
            Nombre = "FERRETERIA EL CAMINANTE",
            RFC = "FER030303003",
            UUID = Guid.NewGuid(),
            CreditoPesos = "20,000",
            CreditoDolares = "1,000",
            RegimenFiscal = "PERSONA MORAL",
            TieneCredito = true,
            DiasCredito = "15",
            DiasGracia = "5",
            SaldoPesos = "10,000",
            SaldoDolares = "500",
            Web = "www.ferretariaelcaminante.com.mx",
            Correo = "contacto@ferretarialugo.com.mx",
            CuentaContable = "0303030303030303",
            FormaPago = "CONTADO"
        };

        protected override async Task OnInitializedAsync()
        {
            TriggerMenuUpdate();
            
            ListaProveedores = new List<ProveedoresDto> { p1, p2, p3 };

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

        private void AbrirDetalleProveedor(ProveedoresDto proveedorSeleccionado, TipoEstadoControl estadoControl)
        {
            string? guidUsuario = proveedorSeleccionado.UUID.ToString();

            // Crear instancia TabNubetico
            TabNubetico tabNubetico = new TabNubetico
            {
                EstadoControl = estadoControl,
                Icono = estadoControl == TipoEstadoControl.Edicion
                            ? $"{MenuItemsFactory.MenuIconDictionary["editar"]}"
                            : this.IconoBase,

                Text = $"{Localizer["Core.ProyectosConstruccion.Proveedor"]} [{proveedorSeleccionado.Nombre}]",
                TipoControl = typeof(ProveedorDetComponent),
                Repetir = true
            };

            // Instanciar componente contenido en TabNubetico
            tabNubetico.Componente = builder =>
            {
                builder.OpenComponent(0, tabNubetico.TipoControl);
                builder.AddAttribute(1, "GuidProveedor", guidUsuario);
                builder.AddAttribute(2, "ProveedorData", proveedorSeleccionado);
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

        private async Task DataGridRowDoubleClick(DataGridRowMouseEventArgs<ProveedoresDto> args)
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

        #endregion

        public class FiltroProveedoresNubeticoGridDto
        {
            public string RFC { get; set; } = "";
            public string Nombre { get; set; } = "";
        }
    }
}
