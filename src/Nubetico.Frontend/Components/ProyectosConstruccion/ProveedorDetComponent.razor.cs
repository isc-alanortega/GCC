using Microsoft.AspNetCore.Components;
using Nubetico.Frontend.Components.Core.Shared;
using Nubetico.Frontend.Models.Enums.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion;

using Nubetico.Frontend.Models.Class.Core;
using Newtonsoft.Json;
using Nubetico.Frontend.Services.Core;
using Nubetico.Shared.Dto.Core;
using System.Globalization;
using Nubetico.Frontend.Models.Static.Core;
using Radzen.Blazor;
using Radzen;
using Nubetico.Shared.Dto.ProyectosConstruccion.Supplies;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Microsoft.Extensions.Localization;

using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Nubetico.Shared.Enums.Core;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class ProveedorDetComponent
    {
        [Parameter]
        public string? GuidProveedor { get; set; }
        [Parameter]
        public ProveedoresDto? ProveedorData { get; set; } // = new ProveedoresDto();
        public TipoEstadoControl EstadoControl { get; set; }

		//[Inject] private GlobalBreakpointService? BreakpointService { get; set; }
		//[Inject] private IStringLocalizer<SharedResources> Localizer { get; set; }

		// ***************************************************************************************************
		// Solo para insumos
		// ***************************************************************************************************
		RadzenDataGrid<InsumosDto>? GridInsumos;
		private bool IsLoading { get; set; } = false;
		private int Count { get; set; }
		private IEnumerable<InsumosDto>? SuppliesList { get; set; }
		[Inject] SuppliesService SuppliesDA { get; set; }
		private SuppliesPaginatedRequestDto RequestForm { get; set; } = new();

		// ***************************************************************************************************

		public bool tieneCredito = true;

        private LotsDetail LotData { get; set; } = new LotsDetail();
        private DomicilioComponent AddressComponent { get; set; }
        //private DomicilioComponent AddressComponent = new DomicilioComponent() { }
        private IDictionary<string, string> IconTabs { get; set; } = new Dictionary<string, string>();

  //      protected override void OnInitialized()
  //      {
  //          breakpointService!.OnChange += StateHasChanged;
  //          base.OnInitialized();

		//	TriggerMenuUpdate();
		//}

  //      protected override async Task OnInitializedAsync()
  //      {
  //          //await LoadMenusPermisosAsync();

  //          //await base.OnInitializedAsync();

		//	TriggerMenuUpdate();
		//}

        //private async Task LoadMenusPermisosAsync()
        //{
        //    var menuPermisosResponse = await MenuService.GetAllMenuWithPermissionsAsync();
        //    if (menuPermisosResponse.Success)
        //    {
        //        this.SelectMenuPermisos = JsonConvert.DeserializeObject<List<MenuPermisosDto>>(menuPermisosResponse.Data.ToString());
        //    }

        //    var sucursalesResponse = await SucursalesService.GetSucursalesAsync();
        //    if (sucursalesResponse.Success)
        //    {
        //        var sucursalesApi = JsonConvert.DeserializeObject<List<SucursalDto>>(sucursalesResponse.Data.ToString());

        //        var currentCulture = CultureInfo.CurrentCulture.Name;
        //        this.SucursalesList.Add(new SucursalDto { IdSucursal = -1, Descripcion = currentCulture == "en-US" ? "ALL" : "TODAS" });
        //        this.SucursalesList.AddRange(sucursalesApi);
        //    }
        //}

        //public void Dispose()
        //{
        //    breakpointService!.OnChange -= StateHasChanged;
        //}



        private bool GetDisabled(string? field_name = null)
        {
            //return EstadoControl == TipoEstadoControl.Lectura;
            return false;
        }

        //public int GetColumnsSize(string? field_name = null)
        //{
        //    var breakpoint = breakpointService.GetCurrentBreakpoint();
        //    if (field_name == "BETWEENSTREET")
        //    {
        //        switch (breakpoint)
        //        {
        //            case Breakpoint.Xs:
        //                return 12;
        //            case Breakpoint.Sm:
        //            case Breakpoint.Md:
        //                return 6;
        //            default:
        //                return 5;
        //        }
        //    }
        //    else
        //    {
        //        switch (breakpoint)
        //        {
        //            case Breakpoint.Xs:
        //                return 12;
        //            case Breakpoint.Sm:
        //            case Breakpoint.Md:
        //                return 6;
        //            default:
        //                return 4;
        //        }
        //    }
        //}

        private string GetCustomValue(string field_name)
        {
            switch (field_name)
            {
                case "SURFACEMEASURE":
                    return LotData.SurfaceMeasure.HasValue ? $"{LotData.SurfaceMeasure?.ToString("0.###")} m2" : "0 m2";
                case "GENERALTAB":
                    bool icon = IconTabs.TryGetValue(field_name, out string? generalkvp);
                    return string.IsNullOrEmpty(generalkvp) ? "" : generalkvp;
                case "LOCATIONTAB":
                    icon = IconTabs.TryGetValue(field_name, out string? locationkvp);
                    return string.IsNullOrEmpty(locationkvp) ? "" : locationkvp;
                default:
                    return "";
            }
        }

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
                    menu.Click = onClick;
                    menusMostrar.Add(menu);
                }
            }

            switch (this.EstadoControl)
            {
                case TipoEstadoControl.Alta:
                    AgregarMenuSiExiste(BaseMenuCommands.SAVE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickSave));
                    AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));
                    break;
                case TipoEstadoControl.Edicion:
                    AgregarMenuSiExiste(BaseMenuCommands.SAVE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickSave));
                    AgregarMenuSiExiste(BaseMenuCommands.CANCEL, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickCancel));
                    AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));
                    break;
                default:
                    AgregarMenuSiExiste(BaseMenuCommands.EDIT, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickEdit));
                    AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));
                    break;

            }

            return menusMostrar;
        }

        private async void OnClickSave()
        {
            //if (!IsValidForm() || IsSaving) return;

            //IsSaving = true;

            //await HandleSaveAsync();

            ////SuppliesDA.NotifyStateChanged();
            //StateHasChanged();
            //IsSaving = false;
        }

        private void OnClickEdit()
        {
            //if (this.EstadoControl == TipoEstadoControl.Edicion) return;

            //UpdateTab(state: TipoEstadoControl.Edicion);
        }

        private async void OnClickCancel()
        {
            //UpdateTab(state: TipoEstadoControl.Lectura);

            //if (this.EstadoControl != TipoEstadoControl.Edicion) return;

            //var response = await SuppliesDA.GetSuppliesById(SupplyData.SuppliesId!.Value);
            //if (!response!.Success || response.Data is null) return;

            //SupplyData = response.Data;
        }

        private void UpdateTab(TipoEstadoControl state)
        {
            this.EstadoControl = state;
            SetNombreTabNubetico($"{Localizer!["Core.ProyectosConstruccion.Proveedor"]} [{ProveedorData.Nombre}]");
            this.TriggerMenuUpdate();
            StateHasChanged();
        }

        private void OnClickClose() => this.CerrarTabNubetico();


		// ***********************************************************************************************
		// Solo para insumos
		// ***********************************************************************************************

		public async Task LoadData(LoadDataArgs args)
		{
			if (IsLoading) return;

			try
			{
				// Procesar ordenamiento
				// Configurar paginación y ordenamiento
				var limit = args.Top ?? 20;
				var offset = args.Skip ?? 0;
				var orderBy = args.Sorts != null && args.Sorts.Any()
					? string.Join(",", args.Sorts.Select(s => $"{s.Property} {(s.SortOrder == SortOrder.Descending ? "desc" : "asc")}"))
					: "Code asc";


				RequestForm.Limit = limit;
				RequestForm.Offset = offset;
				// Obtener datos paginados
				var result = await SuppliesDA.GetPaginatedSupplies(request: RequestForm);

				if (result != null && result.Success && result.Data != null)
				{
					//SuppliesList = result.Data!.Data;
                    var insumos = result.Data!.Data.Where(i => i.Type != "MANO DE OBRA").ToList();
					SuppliesList = insumos;
					Count = result.Data!.RecordsTotal;
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Error al cargar datos: {ex.Message}");
			}
			finally
			{
				IsLoading = false;
				StateHasChanged();
			}
		}

		private BadgeStyle GetBadgeStyle(int id_type) => id_type switch
		{
			2 => BadgeStyle.Info,
			3 => BadgeStyle.Warning,
			_ => BadgeStyle.Base,
		};

		// ***********************************************************************************************

	}
}
