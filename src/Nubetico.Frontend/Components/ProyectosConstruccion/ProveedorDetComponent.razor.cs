using Microsoft.AspNetCore.Components;
using Nubetico.Frontend.Components.Core.Shared;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Microsoft.Extensions.Localization;

using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Nubetico.Shared.Enums.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;
using Microsoft.AspNetCore.Components.Web;
using System.Reflection.Metadata.Ecma335;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Supplies;
using Radzen;
using Radzen.Blazor;
using Nubetico.Frontend.Services.Core;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Frontend.Models.Class.Core;
using Newtonsoft.Json;
using Nubetico.Shared.Dto.Core;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class ProveedorDetComponent : NbBaseComponent
    {
		#region Parametros
		[Parameter]
        public string? GuidProveedor { get; set; }
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected MenuService MenuService { get; set; }
        [Parameter]
        public ProveedorGetDto ProveedorData { get; set; } // = new ProveedoresDto();
        [Inject] private ProveedorServices proveedorService { get; set; }

        #endregion
        #region Propiedades
        private ProveedoresDto? ProveedorDto { get; set; } = new ProveedoresDto();
        private EntidadComponent EntidadComponent { get; set; }

        private bool IsSaving { get; set; } = false;
		private LotsDetail LotData { get; set; } = new LotsDetail();
        private DomicilioComponent AddressComponent { get; set; }
        //private DomicilioComponent AddressComponent = new DomicilioComponent();
		private IDictionary<string, string> IconTabs { get; set; } = new Dictionary<string, string>();
		#endregion

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



        protected override void OnInitialized()
        {
            breakpointService!.OnChange += StateHasChanged;
            base.OnInitialized();

        }

        protected override async Task OnInitializedAsync()
        {

            // Si no viene el ProveedorData, inicializa vacío
            ProveedorData ??= new ProveedorGetDto();

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

        public void Dispose()
        {
            breakpointService!.OnChange -= StateHasChanged;
        }



        private bool GetDisabled(string? field_name = null)
        {
            return EstadoControl == TipoEstadoControl.Lectura;
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

        public async Task RefreshProveedorAsync()
        {
            if (string.IsNullOrEmpty(GuidProveedor))
            {
                this.ProveedorData = new ProveedorGetDto();
                return;
            }

            var proveedorGetResponse = await proveedorService.GetProveedorByIdAsync(ProveedorData.IdProveedor);

            if (proveedorGetResponse != null)
            {

                this.ProveedorData = JsonConvert.DeserializeObject<ProveedorGetDto>(proveedorGetResponse.ToString());

                // RadzenTree requiere que se le proporcione los menus a renderizar
                //this.SelectMenus = this.UsuarioDto.ListaMenus;

                // y en esta otra propiedad los que ya están seleccionados usando como base la misma lista completa
                //this.CheckedMenuDtoValues = RecursiveMenuDtoFilter(this.UsuarioDto.ListaMenus).ToList();
            }
        }
        #region SAVE
        private async void OnClickSave()
        {
            //if (!IsValidForm() || IsSaving) return;
            // 1) Validar ENTIDAD
            if (!EntidadComponent.ValidateProveedor())
            {
                var errorMessages = EntidadComponent.FormValidationErrors
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
            // IsSaving = true;
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
                // Mapear ProveedorGetDto a ProveedorRequestDto
                var proveedorRequest = new ProveedorSaveDto
                {
                    UUID = Guid.NewGuid(),
                    //Aunque el usuario mande algo en ProveedorData... El sp se encarga de foliar
                    Folio = ProveedorData?.Folio ?? string.Empty,
                    Rfc = ProveedorData?.Rfc ?? string.Empty,
                    RazonSocial = ProveedorData?.RazonSocial ?? string.Empty,
                    NombreComercial = ProveedorData?.NombreComercial ?? string.Empty,
                    Email = ProveedorData?.Email ?? string.Empty,
                    Web = ProveedorData?.Web ?? string.Empty,
                    Credito = ProveedorData?.Credito ?? false,
                    Telefono = ProveedorData?.Telefono ?? string.Empty,
                    Celular = ProveedorData?.Celular ?? string.Empty,
                    LimiteCreditoMXN = ProveedorData?.LimiteCreditoMXN ?? 0,
                    LimiteCreditoUSD = ProveedorData?.LimiteCreditoUSD ?? 0,
                    DiasCredito = ProveedorData?.DiasCredito ?? 0,
                    DiasGracia = ProveedorData?.DiasGracia ?? 0,
                    CuentaContable = ProveedorData?.CuentaContable ?? string.Empty,
                    IdFormaPago = ProveedorData?.IdFormaPago ?? 0,
                    Habilitado = ProveedorData?.Habilitado ?? false,
                    IdRegimenFiscal = ProveedorData?.IdRegimenFiscal ?? 0,
                    FechaAlta = ProveedorData?.FechaAlta ?? DateTime.Now,
                    // Nuevos campos de EntidadComponent
                    IdTipoPersonaSat = ProveedorData?.IdTipoPersonaSat ?? 0,
                    IdTipoProveedor = ProveedorData?.IdTipoProveedor ?? 0,
                    IdTipoRegimenFiscal = ProveedorData?.IdTipoRegimenFiscal ?? 0,
                    IdTipoMetodoPago = ProveedorData?.IdTipoMetodoPago ?? 0,
                    IdUsoCFDI = ProveedorData?.IdUsoCFDI ?? 0
                };
                var response = await proveedorService.PostSaveProveedor(proveedorRequest);
                if (!response.Success)
                {
                    // 400: Validación del Proveedor
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
                this.GuidProveedor = response.Data.UUID.ToString();
                this.EstadoControl = TipoEstadoControl.Lectura;
                this.SetNombreTabNubetico($"{Localizer["Core.ProyectosConstruccion.Proveedor"]} [{response.Data.NombreComercial}]");
                this.TriggerMenuUpdate();

                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = Localizer["Shared.Textos.Exito"], Detail = Localizer["Shared.Textos.RegistroGuardado"], Duration = 10000 });
            }
            else if (this.EstadoControl == TipoEstadoControl.Edicion)
            {
                // Mapear ProveedorGetDto a ProveedorRequestDto
                var proveedorRequest = new ProveedorSaveDto
                {
                    UUID = ProveedorData?.UUID ?? Guid.Empty,
                    Folio = ProveedorData?.Folio ?? string.Empty,
                    Rfc = ProveedorData?.Rfc ?? string.Empty,
                    RazonSocial = ProveedorData?.RazonSocial ?? string.Empty,
                    NombreComercial = ProveedorData?.NombreComercial ?? string.Empty,
                    Email = ProveedorData?.Email ?? string.Empty,
                    Web = ProveedorData?.Web ?? string.Empty,
                    Credito = ProveedorData?.Credito ?? false,
                    Telefono = ProveedorData?.Telefono ?? string.Empty,
                    Celular = ProveedorData?.Celular ?? string.Empty,
                    LimiteCreditoMXN = ProveedorData?.LimiteCreditoMXN ?? 0,
                    LimiteCreditoUSD = ProveedorData?.LimiteCreditoUSD ?? 0,
                    DiasCredito = ProveedorData?.DiasCredito ?? 0,
                    DiasGracia = ProveedorData?.DiasGracia ?? 0,
                    CuentaContable = ProveedorData?.CuentaContable ?? string.Empty,
                    IdFormaPago = ProveedorData?.IdFormaPago ?? 0,
                    Habilitado = ProveedorData?.Habilitado ?? false,
                    IdRegimenFiscal = ProveedorData?.IdRegimenFiscal ?? 0,
                    FechaAlta = ProveedorData?.FechaAlta ?? DateTime.Now,
                    // Nuevos campos de EntidadComponent
                    IdTipoPersonaSat = ProveedorData?.IdTipoPersonaSat ?? 0,
                    IdTipoProveedor = ProveedorData?.IdTipoProveedor ?? 0,
                    IdTipoRegimenFiscal = ProveedorData?.IdTipoRegimenFiscal ?? 0,
                    IdTipoMetodoPago = ProveedorData?.IdTipoMetodoPago ?? 0,
                    IdUsoCFDI = ProveedorData?.IdUsoCFDI ?? 0
                };
                var response = await proveedorService.PutSaveProveedor(proveedorRequest);
                if (!response.Success)
                {
                    // 400: Validación del Proveedor
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
                this.GuidProveedor = response.Data.UUID.ToString();
                this.EstadoControl = TipoEstadoControl.Lectura;
                this.SetNombreTabNubetico($"{Localizer["Core.ProyectosConstruccion.Proveedor"]} [{response.Data.NombreComercial}]");
                this.TriggerMenuUpdate();

                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = Localizer["Shared.Textos.Exito"], Detail = Localizer["Shared.Textos.RegistroGuardado"], Duration = 10000 });
            }


            ////SuppliesDA.NotifyStateChanged();
            //StateHasChanged();
            IsSaving = false;
        }
        #endregion


        private void OnClickEdit(MouseEventArgs args)
        {
            this.EstadoControl = TipoEstadoControl.Edicion;
            this.SetNombreTabNubetico($"Proveedor [{this.ProveedorData?.NombreComercial}]");
            this.TriggerMenuUpdate();
        }

        private async void OnClickCancel(MouseEventArgs args)
        {
            //UpdateTab(state: TipoEstadoControl.Lectura);

            //if (this.EstadoControl != TipoEstadoControl.Edicion) return;

            //var response = await SuppliesDA.GetSuppliesById(SupplyData.SuppliesId!.Value);
            //if (!response!.Success || response.Data is null) return;

            //SupplyData = response.Data;
            //  await this.RefreshProveedorAsync();

            this.EstadoControl = TipoEstadoControl.Lectura;
            this.SetNombreTabNubetico($"Proveedor [{this.ProveedorData?.NombreComercial}]");
            this.TriggerMenuUpdate();
        }

        private void UpdateTab(TipoEstadoControl state)
        {
            this.EstadoControl = state;
            SetNombreTabNubetico($"{Localizer!["Core.ProyectosConstruccion.Proveedor"]} [{ProveedorData.NombreComercial}]");
            this.TriggerMenuUpdate();
            StateHasChanged();
        }

        private void OnClickClose(MouseEventArgs args)
        {
            this.CerrarTabNubetico();
        }  

		#region Funciones
		//private async Task<bool> HandleSaveAsync()
		//{
		//	var result = await (this.EstadoControl switch
		//	{
		//		TipoEstadoControl.Alta => proveedorService.PostSaveProveedor(ProveedorData),
		//		//TipoEstadoControl.Edicion => SuppliesDA.PatchEditSupplyAsync(supply: ProveedorData!),
		//		_ => Task.FromResult<BaseResponseDto<ProveedoresDto?>>(null)
		//	});

		//	if (result.StatusCode > 300)
		//	{
		//		var message = result.StatusCode > 400 ? Localizer!["Shared.Text.UnknowError"] : result.Message;
  //              ShowErrorNotification(Localizer!["Shared.Text.ProblemOcurred"], message);

  //              //NotifyAcces(Localizer!["Shared.Text.ProblemOcurred"], message, NotificationSeverity.Error);
  //              return false;
		//	}
  //          ShowInfoNotification(Localizer!["Shared.Text.SaveSucces"]);

  //          //NotifyAcces(string.Empty, Localizer!["Shared.Text.SaveSucces"], NotificationSeverity.Success);

  //          UpdateTab(state: TipoEstadoControl.Lectura);
  //          return true;
		//}
        /// <summary>
        /// Itera de forma recursiva los nodos del menu y devuelve aquellos que la propiedad Check = true o que la totalidad de los nodos tengan chek = true si seleccionable = false
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public IEnumerable<MenuDto> RecursiveMenuDtoFilter(IEnumerable<MenuDto> menus)
        {
            return menus
                .Where(menu =>
                    (menu.Check && menu.Seleccionable)
                    || (!menu.Seleccionable && GetMenuDtoChildrenIsCheck(menu))
                )
                .Concat(
                    menus
                    .Where(menu => menu.Children != null && menu.Children.Any())
                    .SelectMany(menu => RecursiveMenuDtoFilter(menu.Children))
                );
        }

        /// <summary>
        /// Valida si todos los nodos hijos de MenuDto tienen propiedad Check = true
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        private bool GetMenuDtoChildrenIsCheck(MenuDto menu)
        {
            if (menu.Children == null || !menu.Children.Any())
                return false;

            return menu.Children.All(child =>
                (child.Check || !child.Seleccionable && GetMenuDtoChildrenIsCheck(child))
            );
        }
        #endregion

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

        private void ShowInfoNotification(string message) => Notify(Localizer["Subdivisions.Notify.Info"], Localizer[message], NotificationSeverity.Info);

        private void ShowErrorNotification(string message, string? additionalMessage = null)
        {
            string detail = string.IsNullOrEmpty(additionalMessage)
                    ? Localizer[message]
                    : string.Concat($"{Localizer[message]}\n", $"{Localizer["Excel.Text.Row"]} {additionalMessage}.");

            Notify("Error", detail, NotificationSeverity.Error);
        }
        private void Notify(string summary, string? detail, NotificationSeverity severity) => notificationService.Notify(new()
        {
            Severity = severity,
            Summary = summary,
            Detail = detail,
            Duration = 6500,
        });
        // ***********************************************************************************************

    }
}
