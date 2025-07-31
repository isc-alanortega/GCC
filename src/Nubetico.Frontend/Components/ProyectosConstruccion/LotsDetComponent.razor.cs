using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Nubetico.Frontend.Components.Core.Shared;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
	public partial class LotsDetComponent : NbBaseComponent
	{
        [Parameter]
        public string? LotGuid { get; set; }
        [Parameter]
        public LotsCatComponent? ParentCatalog { get; set; }
		[Parameter]
		public int? SubdivisionID { get; set; }
		[Parameter]
		public int? StageID { get; set; }
		[Parameter]
		public int? BlockID { get; set; }

		private LotsDetail LotData { get; set; } = new LotsDetail();
		private RadzenTemplateForm<LotsDetail> LotForm { get; set; }
		RadzenDataGrid<SubdivisionsDto>? Grid { get; set; }
		private IEnumerable<SubdivisionsDto> GeneralList { get; set; }
        private IList<SubdivisionsDto> SelectedSubdivision { get; set; } = new List<SubdivisionsDto>();
		
		private IEnumerable<KeyValuePair<int, string>>? SubdivisionsList { get; set; } = Enumerable.Empty<KeyValuePair<int, string>>();
		private IEnumerable<KeyValuePair<int, string>>? StagesList { get; set; } = Enumerable.Empty<KeyValuePair<int, string>>();
		private IEnumerable<KeyValuePair<int, string>>? BlocksList { get; set; } = Enumerable.Empty<KeyValuePair<int, string>>();
		private RadzenGoogleMap Map { get; set; }
		private DomicilioComponent AddressComponent { get; set; }
		private IDictionary<string, string> IconTabs {  get; set; } = new Dictionary<string, string>();

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();

			LotData.SubdivisionID = SubdivisionID;
			LotData.StageID = StageID;
			LotData.BlockID = BlockID;

            SubdivisionsList = await GetList("SUBDIVISION");
			StagesList = await GetList("STAGE");
			BlocksList = await GetList("BLOCK");

			// Create the values for the icons
			IconTabs = new Dictionary<string, string>
			{
				{ "GENERALTAB", ""},
				{ "LOCATIONTAB", ""}
			};

			await RefreshDetail();
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender && EstadoControl == TipoEstadoControl.Alta)
			{
				LotForm.EditContext.Validate();
			}
		}

		protected override List<RadzenMenuItem> GetMenuItems()
		{
			var baseMenu = MenuItemsFactory.GetBaseMenuItems(Localizer);
			var displayedMenu = new List<RadzenMenuItem>();

			foreach (var option in baseMenu)
			{
				bool command = option.Attributes.TryGetValue("comando", out var commandValue);

				switch (commandValue)
				{
					case "editar":
						// Show only if the status is read
						if (EstadoControl == TipoEstadoControl.Lectura)
						{
							option.Click = EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickEdit);
							displayedMenu.Add(option);
						}
						break;
					case "cerrar":
						option.Click = EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose);
						displayedMenu.Add(option);
						break;
					case "guardar":
						// Show only if the status is different from read
						if (EstadoControl != TipoEstadoControl.Lectura)
						{
							option.Click = EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickSubmit);
							displayedMenu.Add(option);
						}
						break;
					case "cancelar":
						// Show only if the status is edit
						if (EstadoControl == TipoEstadoControl.Edicion)
						{
							option.Click = EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickCancel);
							displayedMenu.Add(option);
						}
						break;
				}
			}

			return displayedMenu;
		}

		private async void OnValueChange(string field_name)
        {
            switch (field_name)
            {
				case "SUBDIVISION":
					LotData.StageID = null;
					LotData.BlockID = null;

					StagesList = await GetList("STAGE");
					BlocksList = await GetList("BLOCK");
					break;
				case "STAGE":
					LotData.BlockID = null;
					BlocksList = await GetList("BLOCK");
					break;
				case "FRONTMEASURE":
                case "BOTTOMMEASURE":
					double front = LotData.FrontMeasure.HasValue ? LotData.FrontMeasure.Value : 0;
					double bottom = LotData.BottomMeasure.HasValue ? LotData.BottomMeasure.Value : 0;

					LotData.SurfaceMeasure = front * bottom;
                    break;
            }

            StateHasChanged();
        }

		private void OnClickEdit()
		{
			EstadoControl = TipoEstadoControl.Edicion;
			SetNombreTabNubetico($"{Localizer["Subdivisions.Text.Lot"]} [{LotData.Number}]");
			TriggerMenuUpdate();
		}

        private async void OnClickSubmit()
        {
			var resultAddress = AddressComponent.SaveAddress();
			if (!LotForm.EditContext.Validate() || !resultAddress.Success)
			{
				IconTabs["LOCATIONTAB"] = !resultAddress.Success ? "error" : "";
				IconTabs["GENERALTAB"] = !LotForm.EditContext.Validate() ? "error" : "";
				StateHasChanged();

				return;
			}

			// Set the object for the Address
			if (resultAddress.Success && resultAddress.Result != null)
			{
				LotData.Address = resultAddress.Result;
			}

			foreach (var key in IconTabs.Keys)
			{
				IconTabs[key] = "";
			}

			var submitResponse = EstadoControl == TipoEstadoControl.Alta
				? await lotsService.PostLot(LotData)
				: await lotsService.UpdateLot(LotData);

			if (submitResponse == null || submitResponse.StatusCode == 500)
			{
				ShowErrorNotification("Lots.Error.SaveLot");
				return;
			}

			if (submitResponse.StatusCode == 201 && !string.IsNullOrEmpty(submitResponse.Message))
			{
				LotGuid = submitResponse.Message;
				await RefreshDetail();
			}
			ShowInfoNotification("Lots.Saved.Lot");

			// Change status control
			EstadoControl = TipoEstadoControl.Lectura;
			SetNombreTabNubetico($"{Localizer["Subdivisions.Text.Lot"]} [{LotData.Number}]");

			// Refresh the Lot Catalog
			if (ParentCatalog != null)
			{
				var args = new LoadDataArgs { Top = 20, Skip = 0 };
				await ParentCatalog.LoadData(args);
			}

			TriggerMenuUpdate();
			StateHasChanged();
		}

		private void OnClickClose()
		{
			CerrarTabNubetico();
		}

		private async void OnClickCancel()
		{
			await RefreshDetail();
			await AddressComponent.RefreshDetail();

			EstadoControl = TipoEstadoControl.Lectura;
			SetNombreTabNubetico($"{Localizer["Subdivisions.Text.Lot"]} [{LotData.Number}]");
			TriggerMenuUpdate();
			
			foreach (var key in IconTabs.Keys)
			{
				IconTabs[key] = "";
			}

			StateHasChanged();
		}

		private void OnClickOpenModel()
		{
			try
			{
				var nubeticoTab = new TabNubetico
				{
					EstadoControl = TipoEstadoControl.Edicion,
					Icono = IconoBase,
					Text = "Nuevo Modelo",
					TipoControl = typeof(ModelsDetComponent),
					Repetir = true
				};

				nubeticoTab.Componente = builder =>
				{
					builder.OpenComponent(0, nubeticoTab.TipoControl);
					builder.AddComponentReferenceCapture(1, instance =>
					{
						if (instance is NbBaseComponent nbComponent)
						{
							nubeticoTab.InstanciaComponente = nbComponent;
							nbComponent.EstadoControl = TipoEstadoControl.Edicion;
							if (!string.IsNullOrEmpty(IconoBase))
							{
								nbComponent.IconoBase = IconoBase;
							}
							nbComponent.TriggerMenuUpdate();
						}
					});
					builder.CloseComponent();
				};
				this.AgregarTabNubetico(nubeticoTab);
			}
			catch
			{
				return;
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

        private async Task<IEnumerable<KeyValuePair<int, string>>?> GetList(string field_name)
        {
			var result = new BaseResponseDto<object>();

			switch (field_name)
			{
				case "SUBDIVISION":
					result = await subdivisionsService.GetSubdivisionsList();
					break;
				case "STAGE":
					result = await subdivisionsService.GetStagesList(LotData.SubdivisionID);
					break;
				case "BLOCK":
					result = await subdivisionsService.GetBlocksList(LotData.SubdivisionID, LotData.StageID);
					break;
				default:
					return Enumerable.Empty<KeyValuePair<int, string>>();
			}

			if (result == null || result.StatusCode != 200)
			{
				return Enumerable.Empty<KeyValuePair<int, string>>();
			}
            
            var list = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<int, string>>>(result.Data.ToString()).ToList();

			return list.OrderBy(items => items.Key);
		}

		private bool GetDisabled(string field_name)
		{
			switch (field_name)
			{
				case "SUBDIVISION":
					return (SubdivisionID.HasValue && EstadoControl != TipoEstadoControl.Lectura) || EstadoControl == TipoEstadoControl.Lectura;
				default:
					return EstadoControl == TipoEstadoControl.Lectura;
			}
		}

		private void OnMapClick(GoogleMapClickEventArgs args)
		{

		}

		private async Task OnMarkerClick(RadzenGoogleMapMarker marker)
		{

		}

		private async Task RefreshDetail()
		{
			if (string.IsNullOrEmpty(LotGuid))
			{
				return;
			}

			var lotGetResponse = await lotsService.GetLotByGuid(LotGuid);
			if (lotGetResponse == null || !lotGetResponse.Success)
			{
				CerrarTabNubetico();
				return;
			}

			LotData = JsonConvert.DeserializeObject<LotsDetail>(lotGetResponse.Data.ToString());
		}

		private void ShowInfoNotification(string message)
		{
			var notification = new NotificationMessage
			{
				Severity = NotificationSeverity.Info,
				Summary = Localizer["Subdivisions.Notify.Info"],
				Detail = Localizer[message],
				Duration = 3500
			};

			notificationService.Notify(notification);
		}

		private void ShowErrorNotification(string message)
		{
			var notification = new NotificationMessage
			{
				Severity = NotificationSeverity.Error,
				Summary = "Error",
				Detail = Localizer[message],
				Duration = 3500
			};

			notificationService.Notify(notification);
		}
	}
}
