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
    public partial class ContratistaCatComponent : NbBaseComponent
    {
        [Inject]
        protected IStringLocalizer<SharedResources> Localizer { get; set; }
        private RadzenDataGrid<ContratistasDto>? GridContratistas { get; set; }
        private List<ContratistasDto>? ListaContratistas { get; set; }
        private int Count { get; set; }
        private bool IsLoading { get; set; } = false;
        private int RowsPerPage { get; set; } = 10;
        public IList<ContratistasDto> ContratistasSeleccionados { get; set; } = new List<ContratistasDto>();
        private FiltroContratistasNubeticoGridDto Filtro { get; set; } = new FiltroContratistasNubeticoGridDto();
        private List<BasicItemSelectDto> SelectEstadosContratista = new List<BasicItemSelectDto>();
        public bool busy { get; set; } = false;

        ContratistasDto cont1 = new ContratistasDto()
        {
            Folio = "CNT01",
            EstadoContratista = "Activo",
            IdEstadoContratista = 1,
            Nombre = "Arq. Vergara",
            RFC = "ARQ010101001",
            UUID = Guid.NewGuid(),
            CreditoPesos = "50,000",
            CreditoDolares = "2,500",
            RegimenFiscal = "PERSONA MORAL",
            TieneCredito = true,
            DiasCredito = "30",
            DiasGracia = "5",
            SaldoPesos = "15,000",
            SaldoDolares = "750",
            Web = "www.vergaraconstrucciones.com.mx",
            Correo = "contacto@vergaraconstrucciones.com.mx",
            CuentaContable = "0101010101010101",
            FormaPago = "TRANSFERENCIA"
        };

        ContratistasDto cont2 = new ContratistasDto() {
            Folio = "CNT02",
            EstadoContratista = "Activo",
            IdEstadoContratista = 1,
            Nombre = "Arq. Ojeda",
            RFC = "ARQ020202002",
            UUID = Guid.NewGuid(),
            CreditoPesos = "75,000",
            CreditoDolares = "3,500",
            RegimenFiscal = "PERSONA MORAL",
            TieneCredito = true,
            DiasCredito = "30",
            DiasGracia = "5",
            SaldoPesos = "18,000",
            SaldoDolares = "900",
            Web = "www.arqojeda.com",
            Correo = "info@arqojeda.com",
            CuentaContable = "0202020202020202",
            FormaPago = "TRANSFERENCIA"
        };

        ContratistasDto cont3 = new ContratistasDto() {
            Folio = "CNT03",
            EstadoContratista = "Inactivo",
            IdEstadoContratista = 0,
            Nombre = "Arq. Perez",
            RFC = "ARQ030303003",
            UUID = Guid.NewGuid(),
            CreditoPesos = "20,000",
            CreditoDolares = "1,000",
            RegimenFiscal = "PERSONA MORAL",
            TieneCredito = true,
            DiasCredito = "15",
            DiasGracia = "5",
            SaldoPesos = "10,000",
            SaldoDolares = "500",
            Web = "www.perezyperez.com.mx",
            Correo = "contacto@perezyperez.com.mx",
            CuentaContable = "0303030303030303",
            FormaPago = "CONTADO"
        };

        protected override async Task OnInitializedAsync()
        {
            TriggerMenuUpdate();

            ListaContratistas = new List<ContratistasDto> { cont1, cont2, cont3 };
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
        }

        private void OnAbrirClick(MouseEventArgs args)
        {
            if (ContratistasSeleccionados.Count == 0)
                return;

            var contratistaSeleccionado = ContratistasSeleccionados.FirstOrDefault();

            if (contratistaSeleccionado != null)
                AbrirDetalleContratista(contratistaSeleccionado, TipoEstadoControl.Lectura);
        }

        private void OnEditarClick(MouseEventArgs args)
        {
            if (ContratistasSeleccionados.Count == 0)
                return;

            var contratistaSeleccionado = ContratistasSeleccionados.FirstOrDefault();

            if (contratistaSeleccionado != null)
                AbrirDetalleContratista(contratistaSeleccionado, TipoEstadoControl.Edicion);
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

        private void AbrirDetalleContratista(ContratistasDto contratistaSeleccionado, TipoEstadoControl estadoControl)
        {
            string? guidContratista = contratistaSeleccionado.UUID.ToString();

            // Crear instancia TabNubetico
            TabNubetico tabNubetico = new TabNubetico
            {
                EstadoControl = estadoControl,
                Icono = estadoControl == TipoEstadoControl.Edicion
                            ? MenuItemsFactory.MenuIconDictionary["editar"]
                            : this.IconoBase,

                Text = $"{Localizer["Core.ProyectosConstruccion.Contratista"]} [{contratistaSeleccionado.Nombre}]",
                TipoControl = typeof(ContratistaDetComponent),
                Repetir = true
            };

            // Instanciar componente contenido en TabNubetico
            tabNubetico.Componente = builder =>
            {
                builder.OpenComponent(0, tabNubetico.TipoControl);
                builder.AddAttribute(1, "GuidContratista", guidContratista);
                builder.AddAttribute(2, "ContratistaData", contratistaSeleccionado);
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

        private async Task DataGridRowDoubleClick(DataGridRowMouseEventArgs<ContratistasDto> args)
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

        public class FiltroContratistasNubeticoGridDto
        {
            public string RFC { get; set; } = "";
            public string Nombre { get; set; } = "";
        }
    }
}
