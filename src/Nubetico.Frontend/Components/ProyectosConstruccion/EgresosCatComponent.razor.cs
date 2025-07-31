using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.Palmaterra;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Palmaterra;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Supplies;
using Radzen;
using Radzen.Blazor;
using static Nubetico.Frontend.Components.ProyectosConstruccion.ProveedorCatComponent;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class EgresosCatComponent : NbBaseComponent
    {
        [Inject]
        protected IStringLocalizer<SharedResources> Localizer { get; set; }
        private RadzenDataGrid<vEgresos_Partidas_DetallesDto>? GridEgresos { get; set; }
        private List<vEgresos_Partidas_DetallesDto>? ListaEgresos { get; set; }
        //private IEnumerable<GroupDescriptor> defaultGroup;
        private bool AgrupasRenglones = true;
        private int Count { get; set; }
        private bool IsLoading { get; set; } = false;
        private int RowsPerPage { get; set; } = 10;
        //public IList<ProveedoresDto> ProveedoresSeleccionados { get; set; } = new List<ProveedoresDto>();
        public IList<vEgresos_Partidas_DetallesDto> EgresosSeleccionados { get; set; } = [];
        [Inject] EgresosService egresosService { get; set; }
		[Inject]ObrasServices obrasService { get; set; }


		private FiltroProveedoresNubeticoGridDto Filtro { get; set; } = new FiltroProveedoresNubeticoGridDto();

        protected override async Task OnInitializedAsync()
        {
            TriggerMenuUpdate();
            var result2 = await egresosService.GetvEgresos_Partidas_DetallesAsync();
            var resX = JsonConvert.SerializeObject(result2.Data);
            ListaEgresos = JsonConvert.DeserializeObject<List<vEgresos_Partidas_DetallesDto>>(resX);

            GridEgresos.Groups.Clear();
            GridEgresos.Groups.Add(new GroupDescriptor() { Property = "Id_Obra", SortOrder = SortOrder.Ascending, Title = "Id_Obra" });
            StateHasChanged();
            await GridEgresos.Reload();
            AgrupasRenglones = false;

			var resultObras = await obrasService.GetObrasAsync();
			var resObras = JsonConvert.SerializeObject(resultObras.Data);
			List<ObrasDto>? ListaObras = JsonConvert.DeserializeObject<List<ObrasDto>>(resX);
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

        private async void OnAgregarClick(MouseEventArgs args)
        {
			//var result = await obrasService.GetObrasAsync();
			//var resX = JsonConvert.SerializeObject(result.Data);
			//List<ObrasDto>? ListaObras = JsonConvert.DeserializeObject<List<ObrasDto>>(resX);

		//string? guidProveedor = null;

		//// Crear instancia TabNubetico
		//TabNubetico tabNubetico = new TabNubetico
		//{
		//    EstadoControl = TipoEstadoControl.Alta,
		//    Icono = $"{MenuItemsFactory.MenuIconDictionary["agregar"]}",
		//    Text = Localizer["Core.ProyectosConstruccion.ProveedoresAgregar"],
		//    TipoControl = typeof(ProveedorDetComponent),
		//    Repetir = true
		//};

		//// Instanciar componente contenido en TabNubetico
		//tabNubetico.Componente = builder =>
		//{
		//    builder.OpenComponent(0, tabNubetico.TipoControl);
		//    builder.AddAttribute(1, "GuidProveedor", guidProveedor);
		//    builder.AddComponentReferenceCapture(1, instance =>
		//    {
		//        // Asegurarnos que el componente interno instanciado hereda el componente base
		//        if (instance is NbBaseComponent nbComponent)
		//        {
		//            tabNubetico.InstanciaComponente = nbComponent;
		//            // Establecer el menú inicial para el componente
		//            nbComponent.EstadoControl = TipoEstadoControl.Alta;

		//            if (!string.IsNullOrEmpty(this.IconoBase))
		//                nbComponent.IconoBase = this.IconoBase;

		//            nbComponent.TriggerMenuUpdate();
		//        }
		//    });
		//    builder.CloseComponent();
		//};

		//this.AgregarTabNubetico(tabNubetico);
	    }

        private void OnAbrirClick(MouseEventArgs args)
        {
            //if (ProveedoresSeleccionados.Count == 0)
            //    return;

            //var proveedorSeleccionado = ProveedoresSeleccionados.FirstOrDefault();

            //if (proveedorSeleccionado != null)
            //    AbrirDetalleProveedor(proveedorSeleccionado, TipoEstadoControl.Lectura);
        }

        private void OnEditarClick(MouseEventArgs args)
        {
            //if (ProveedoresSeleccionados.Count == 0)
            //    return;

            //var proveedorSeleccionado = ProveedoresSeleccionados.FirstOrDefault();

            //if (proveedorSeleccionado != null)
            //    AbrirDetalleProveedor(proveedorSeleccionado, TipoEstadoControl.Edicion);
        }

        private async Task OnRefrescarClickAsync(MouseEventArgs args)
        {
            //await RefreshGridAsync("NombreCompleto asc", this.RowsPerPage, 0);
        }

        private void OnCerrarClick(MouseEventArgs args)
        {
            this.CerrarTabNubetico();
        }

        #endregion

        #region funciones

        private void AbrirDetalleEgreso(EgresoDto egresoSeleccionado, TipoEstadoControl estadoControl)
        {
            //string? guidUsuario = proveedorSeleccionado.UUID.ToString();

            //// Crear instancia TabNubetico
            //TabNubetico tabNubetico = new TabNubetico
            //{
            //    EstadoControl = estadoControl,
            //    Icono = estadoControl == TipoEstadoControl.Edicion
            //                ? $"{MenuItemsFactory.MenuIconDictionary["editar"]}"
            //                : this.IconoBase,

            //    Text = $"{Localizer["Core.ProyectosConstruccion.Proveedor"]} [{proveedorSeleccionado.Nombre}]",
            //    TipoControl = typeof(ProveedorDetComponent),
            //    Repetir = true
            //};

            //// Instanciar componente contenido en TabNubetico
            //tabNubetico.Componente = builder =>
            //{
            //    builder.OpenComponent(0, tabNubetico.TipoControl);
            //    builder.AddAttribute(1, "GuidProveedor", guidUsuario);
            //    builder.AddAttribute(2, "ProveedorData", proveedorSeleccionado);
            //    builder.AddComponentReferenceCapture(1, instance =>
            //    {
            //        // Asegurarnos que el componente interno instanciado hereda el componente base
            //        if (instance is NbBaseComponent nbComponent)
            //        {
            //            tabNubetico.InstanciaComponente = nbComponent;
            //            // Establecer el estado inicial para el componente
            //            nbComponent.EstadoControl = estadoControl;

            //            if (!string.IsNullOrEmpty(this.IconoBase))
            //                nbComponent.IconoBase = this.IconoBase;

            //            nbComponent.TriggerMenuUpdate();
            //        }
            //    });
            //    builder.CloseComponent();
            //};

            //this.AgregarTabNubetico(tabNubetico);
        }

        private async Task DataGridRowDoubleClick(DataGridRowMouseEventArgs<vEgresos_Partidas_DetallesDto> args)
        {
            //if (args.Data != null)
            //    AbrirDetalleProveedor(args.Data, TipoEstadoControl.Lectura);
        }

        private async Task LoadDataAsync(LoadDataArgs args)
        {
            //string orderBy = string.Join(",", args.Sorts.Select(s => $"{s.Property} {(s.SortOrder == SortOrder.Descending ? "desc" : "asc")}"));
            //await RefreshGridAsync(orderBy, args.Top ?? 0, args.Skip ?? 0);
        }

        void OnGroupRowRender(GroupRowRenderEventArgs args)
        {
            if (AgrupasRenglones)
            {
                args.Expanded = false; // Los grupos estarán colapsados por defecto
            }
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
    }
}
