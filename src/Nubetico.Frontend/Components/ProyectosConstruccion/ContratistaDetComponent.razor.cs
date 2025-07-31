using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using Nubetico.Frontend.Components.Core.Shared;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Frontend.Models.Enums.Core;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.Core;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Contratistas;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;
using Nubetico.Shared.Dto.ProyectosConstruccion.Supplies;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class ContratistaDetComponent : NbBaseComponent
    {
        [Parameter]
        public string? GuidContratista { get; set; }
        [Inject] protected DialogService DialogService { get; set; }

        [Inject]
        protected MenuService MenuService { get; set; }

        [Parameter]
        public ContratistasDto ContratistaData { get; set; }
        [Inject] protected ContratistaService ContratistasService { get; set; }
        private RadzenTemplateForm<ContratistasDto> GeneralContratistasForm { get; set; }
        private EntidadContratistaComponent EntidadContratistaComponent { get; set; }


        [Inject] protected EntidadesService entidadesService { get; set; }
        [Inject] protected SuppliesService insumosService { get; set; }
        private List<TablaRelacionDto> LstRegimenesFiscales = new();
        private List<TablaRelacionDto> LstTipoRegimenesFiscales = new();
        private List<TablaRelacionDto> LstTipoFormaPago = new();
        private List<TablaRelacionDto> LstTipoInsumo = new();
        private List<TablaRelacionStringDto> LstUsoCFDI = new();
        private List<TablaRelacionStringDto> LstTipoMetodoPago = new();
        private bool IsCreditEnabled => ContratistaData.Credito;


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

		//public bool tieneCredito = true;

        private LotsDetail LotData { get; set; } = new LotsDetail();
        private DomicilioComponent AddressComponent { get; set; }
        private IDictionary<string, string> IconTabs { get; set; } = new Dictionary<string, string>();

        protected override void OnInitialized()
        {
            breakpointService!.OnChange += StateHasChanged;
            base.OnInitialized();
        }
        protected override async Task OnInitializedAsync()
        {

            // Si no viene el ProveedorData, inicializa vacío
            ContratistaData ??= new ContratistasDto();
            LstRegimenesFiscales = await entidadesService.GetAllTipoRegimenFiscal();
            LstTipoRegimenesFiscales = await entidadesService.GetAllRegimenFiscal();
            LstTipoFormaPago = await entidadesService.GetAllFormaPago();
            LstTipoMetodoPago = await entidadesService.GetAllMetodoDePago();
            LstTipoInsumo = await insumosService.GetAllTipoInsumo();
            LstUsoCFDI = await entidadesService.GetAllUsoCFDI();
        }
        //Con esto el componente carga correctamente el menú superior
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                TriggerMenuUpdate();    //Refleja el estado de control
                StateHasChanged();
            }
        }

        #region MENU
        protected override List<RadzenMenuItem> GetMenuItems()
        {
            var menusDefinidos = MenuItemsFactory.GetBaseMenuItems(Localizer);
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
                    StateHasChanged();
                }
            }

            if (this.EstadoControl == TipoEstadoControl.Alta)
            {
                AgregarMenuSiExiste(BaseMenuCommands.SAVE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickSave));
                AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));
            }
            else if (this.EstadoControl == TipoEstadoControl.Edicion)
            {
                AgregarMenuSiExiste(BaseMenuCommands.SAVE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickSave));
                AgregarMenuSiExiste(BaseMenuCommands.CANCEL, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickCancel));
                AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));
            }
            else if (this.EstadoControl == TipoEstadoControl.Lectura)
            {
                AgregarMenuSiExiste(BaseMenuCommands.EDIT, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickEdit));
                AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));
            }

            return menusMostrar;
        }
        #endregion

        #region ACTIONS_MENU
        private async void OnClickSave()
        {
            if (!EntidadContratistaComponent.ValidateContratista())
            {
                var errorMessages = EntidadContratistaComponent.FormValidationErrors
                    .SelectMany(kvp => kvp.Value.Select(msg => $"{kvp.Key}: {msg}"))
                    .ToList();
                var errorDetail = errorMessages.Any()
                    ? string.Join("; ", errorMessages)
                    : Localizer["Core.ProyectosConstruccion.InvalidForm"];

                ShowNotification(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = errorDetail,
                    Duration = 10000
                });
                return;
            }
            bool? dialogResult = await DialogService.Confirm(
                 Localizer["Shared.Dialog.SaveChanges"],
                 Localizer["Shared.Dialog.Atencion"],
                 new ConfirmOptions
                 {
                     OkButtonText = Localizer["Shared.Botones.Aceptar"],
                     CancelButtonText = Localizer["Shared.Botones.Cancelar"]
                 }
             );
            if (dialogResult != true) return;
            if (this.EstadoControl == TipoEstadoControl.Alta)
            {
                ContratistaData.IdUsuarioAlta = 42;
                ContratistaData.UUID = Guid.NewGuid();
                ContratistaData.FechaAlta = DateTime.Now;
                ContratistaData.Folio = "";
                 var response = await ContratistasService.PostSaveContratista(ContratistaData);
                if (!response.Success)
                {
                    // 400: Validación del Proveedor
                    if (response.StatusCode == 400)
                    {
                        ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error al Guardar Contratista", Detail = "Compruebe los campos requeridos", Duration = 10000 });
                        return;
                    }

                    // 500: Error en servidor
                    if (response.StatusCode == 500)
                    {
                        ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = response.Message, Duration = 10000 });
                        return;
                    }

                    return;
                }
                this.GuidContratista = response.Data.UUID.ToString();
                this.EstadoControl = TipoEstadoControl.Lectura;
                this.SetNombreTabNubetico($"{Localizer["Core.ProyectosConstruccion.Contratista"]} [{response.Data.NombreComercial}]");
                this.TriggerMenuUpdate();

                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = Localizer["Shared.Textos.Exito"], Detail = Localizer["Shared.Textos.RegistroGuardado"], Duration = 10000 });
            }
            else if (this.EstadoControl == TipoEstadoControl.Edicion)
            {
                
                var response = await ContratistasService.PutSaveContratista(ContratistaData);
                if (!response.Success)
                {
                    // 400: Validación del Contratista
                    if (response.StatusCode == 400)
                    {
                        var errores = JsonConvert.DeserializeObject<List<ValidationFailureDto>>(response.Data.ToString());
                        if (errores != null)
                            ReadFormValidationErrors(errores);

                        return;
                    }

                    // 500: Error en servidor
                    if (response.StatusCode == 500)
                    {
                        ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = response.Message, Duration = 10000 });
                        return;
                    }

                    return;
                }
                this.GuidContratista = response.Data.UUID.ToString();
                this.EstadoControl = TipoEstadoControl.Lectura;
                this.SetNombreTabNubetico($"{Localizer["Core.ProyectosConstruccion.Contratista"]} [{response.Data.NombreComercial}]");
                this.TriggerMenuUpdate();

                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = Localizer["Shared.Textos.Exito"], Detail = Localizer["Shared.Textos.RegistroGuardado"], Duration = 10000 });
            }

        }

        private void OnClickClose(MouseEventArgs args)
        {
            this.CerrarTabNubetico();
        }

        private async void OnClickCancel(MouseEventArgs args)
        {

            this.EstadoControl = TipoEstadoControl.Lectura;
            this.SetNombreTabNubetico($"Contratista [{this.ContratistaData?.NombreComercial}]");
            this.TriggerMenuUpdate();
        }

        private void OnClickEdit(MouseEventArgs args)
        {
            this.EstadoControl = TipoEstadoControl.Edicion;
            this.SetNombreTabNubetico($"Contratista [{this.ContratistaData?.NombreComercial}]");
            this.TriggerMenuUpdate();
        }

        #endregion
        public void Dispose()
        {
            breakpointService!.OnChange -= StateHasChanged;
        }



        private bool GetDisabled(string? field_name = null)
        {
           return EstadoControl == TipoEstadoControl.Lectura;
           // return false;
        }

        public int GetColumnsSize(string? field_name = null)
        {
            var breakpoint = breakpointService.GetCurrentBreakpoint();
            if (field_name == "BETWEENSTREET")
            {
                switch (breakpoint)
                {
                    case Breakpoint.Xs:
                        return 12;
                    case Breakpoint.Sm:
                    case Breakpoint.Md:
                        return 6;
                    default:
                        return 5;
                }
            }
            else
            {
                switch (breakpoint)
                {
                    case Breakpoint.Xs:
                        return 12;
                    case Breakpoint.Sm:
                    case Breakpoint.Md:
                        return 6;
                    default:
                        return 4;
                }
            }
        }

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
					var insumos = result.Data!.Data.Where(i => i.Type == "MANO DE OBRA").ToList();
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
