using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class LotsCatComponent : NbBaseComponent
    {
		[Parameter]
		public TipoEstadoControl EstadoControl { get; set; } = TipoEstadoControl.Lectura;
		[Parameter]
		public int? SubdivisionFilter { get; set; }
        [Parameter]
        public bool DisabledSubdivisionFilter { get; set; } = false;
        [Parameter]
        public bool VisibleExtraOptions { get; set; } = false;
        
		RadzenDataGrid<LotsDto>? Grid { get; set; }
        private IEnumerable<LotsDto>? LotsList { get; set; }
		private IList<LotsDto> SelectedLot { get; set; } = new List<LotsDto>();
		private FilterLots Filter { get; set; } = new FilterLots();
		private int Count { get; set; }
		private bool IsLoading { get; set; } = false;
		private IEnumerable<KeyValuePair<int, string>> SubdivisionsList { get; set; } = Enumerable.Empty<KeyValuePair<int, string>>();
        private IEnumerable<KeyValuePair<int, string>> StagesList { get; set; } = Enumerable.Empty<KeyValuePair<int, string>>();
        private IEnumerable<KeyValuePair<int, string>> BlocksList { get; set; } = Enumerable.Empty<KeyValuePair<int, string>>();
        private IEnumerable<KeyValuePair<int, string>> ModelsList { get; set; } = Enumerable.Empty<KeyValuePair<int, string>>();
        private IEnumerable<KeyValuePair<int, string>> StatusList { get; set; } = Enumerable.Empty<KeyValuePair<int, string>>();

        protected override async Task OnParametersSetAsync()
        {
			Filter.SubdivisionID = SubdivisionFilter;

            SubdivisionsList = await GetListsAsync("SUBDIVISION");
            StagesList = await GetListsAsync("STAGE");
            BlocksList = await GetListsAsync("BLOCK");

            await LoadData(new LoadDataArgs { Top = 20, Skip = 0 });
			StateHasChanged();
		}

        public async Task LoadData(LoadDataArgs args)
        {
            IsLoading = true;
            var result = await lotsService.GetPaginatedLots(args.Top ?? 0, args.Skip ?? 0, Filter);
            if (!result.Success || result.Data == null)
            {
                IsLoading = false;
                StateHasChanged();

                return;
            }

            var paginatedList = JsonConvert.DeserializeObject<PaginatedListDto<LotsDto>>(result.Data.ToString());
            if (paginatedList != null)
            {
                LotsList = paginatedList.Data;
                Count = paginatedList.RecordsTotal;
            }
			IsLoading = false;
            StateHasChanged();
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
                    case "abrir":
                        option.Click = EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickOpen);
                        displayedMenu.Add(option);
                        break;
                    case "agregar":
                        option.Click = EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickAdd);
                        displayedMenu.Add(option);
                        break;
                    case "editar":
                        option.Click = EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickEdit);
                        displayedMenu.Add(option);
                        break;
                    case "refrescar":
                        option.Click = EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickRefresh);
                        displayedMenu.Add(option);
                        break;
                    case "cerrar":
                        option.Click = EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose);
                        displayedMenu.Add(option);
                        break;
                }
            }

            return displayedMenu;
        }

        private async void OnValueChange(string field_name)
        {
            if (field_name == "SUBDIVISION")
            {
                Filter.StageID = null;
                Filter.BlockID = null;

                StagesList = await GetListsAsync("STAGE");
                BlocksList = await GetListsAsync("BLOCK");
            }
            else if (field_name == "STAGE")
            {
				Filter.BlockID = null;

                BlocksList = await GetListsAsync("BLOCK");
            }

            await LoadData(new LoadDataArgs { Top = 20, Skip = 0 });

            StateHasChanged();
        }

        private void OnClickOpen()
        {
            if (!SelectedLot.Any())
            {
                ShowInfoNotification();
                return;
            }

            OpenDetailControl(TipoEstadoControl.Lectura, IconoBase, SelectedLot.First());
        }

        private void OnClickAdd()
        {
            OpenDetailControl(TipoEstadoControl.Alta, "add");
        }

        private void OnClickPreloadedAdd()
        {
            var lot = new LotsDto { SubdivisionID = SubdivisionFilter };
			OpenDetailControl(TipoEstadoControl.Alta, "add", lot);
		}

        private void OnClickEdit()
        {
			if (!SelectedLot.Any())
			{
				ShowInfoNotification();
				return;
			}

			OpenDetailControl(TipoEstadoControl.Edicion, "edit_square", SelectedLot.First());
		}

        private async void OnClickRefresh()
        {
            var args = new LoadDataArgs { Top = 20, Skip = 0 };
            await LoadData(args);
        }

        private void OnClickClose()
        {
            this.CerrarTabNubetico();
        }

		private async void OnFilterClick()
        {
            var args = new LoadDataArgs { Top = 20, Skip = 0 };
            await LoadData(args);
        }

        private void OnDeleteFiltersClick()
        {
            Filter.SubdivisionID = null;
            Filter.StageID = null;
            Filter.BlockID = null;
            Filter.ModelID = null;
            Filter.LotNumber = null;
            Filter.StatusID = null;
        }

        private async Task<IEnumerable<KeyValuePair<int, string>>> GetListsAsync(string field_name)
        {
            try
            {
                var resultGetResponse = new BaseResponseDto<object>();
                switch (field_name)
                {
                    case "SUBDIVISION":
                        resultGetResponse = await subdivisionsService.GetSubdivisionsList();
						break;
                    case "STAGE":
                        resultGetResponse = await subdivisionsService.GetStagesList(Filter.SubdivisionID);
						break;
                    case "BLOCK":
                        resultGetResponse = await subdivisionsService.GetBlocksList(Filter.SubdivisionID, Filter.StageID);
						break;
                }

                if (resultGetResponse == null || resultGetResponse.StatusCode == 500)
                {
                    return Enumerable.Empty<KeyValuePair<int, string>>();
                }

                var result = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<int, string>>>(resultGetResponse.Data.ToString());
                if (result == null || !result.Any())
                {
					return Enumerable.Empty<KeyValuePair<int, string>>();
				}

				return result.OrderBy(option => option.Value);
			}
            catch
            {
                return Enumerable.Empty<KeyValuePair<int, string>>();
			}
        }

        private bool GetDisabledAddLot()
        {
            return EstadoControl == TipoEstadoControl.Lectura;
        }

        private string GetDynamicStyle()
        {
            return "height: fit-content; max-height: calc(100vh - 455px); min-height: 300px; overflow: auto";

            return $"height: calc(100vh - {(VisibleExtraOptions ? 520 : 470)}px); min-height: 300px; overflow: auto";
        }

        private void DataGridRowDoubleClick(DataGridRowMouseEventArgs<LotsDto> lot)
        {
            if (lot.Data == null)
            {
                return;
            }

            // Open detail in reading status 
            OpenDetailControl(TipoEstadoControl.Lectura, IconoBase, lot.Data);
        }

        private void OpenDetailControl(TipoEstadoControl controlStatus, string iconTab, LotsDto? lot = null)
        {
            try
            {
                // Create new tab
                var nubeticoTab = new TabNubetico
                {
                    EstadoControl = controlStatus,
                    Icono = iconTab,
                    Text = controlStatus == TipoEstadoControl.Alta
                        ? $"{Localizer["Shared.Textos.Nuevo"]} {Localizer["Subdivisions.Text.Lot"]}"
                        : $"{Localizer["Subdivisions.Text.Lot"]} [{lot.Number}]",
                    TipoControl = typeof(LotsDetComponent),
                    Repetir = true
                };

                // Instantiate the component in the tab
                string? lotGuid = lot != null && lot.UUID.HasValue ? lot.UUID.ToString() : null;
                nubeticoTab.Componente = builder =>
                {
                    builder.OpenComponent(0, nubeticoTab.TipoControl);
                    builder.AddAttribute(1, "LotGuid", lotGuid);
                    builder.AddAttribute(2, "ParentCatalog", this);
                    builder.AddAttribute(3, "SubdivisionID", lot != null ? lot.SubdivisionID : null);
					builder.AddAttribute(4, "StageID", lot != null ? lot.StageID : null);
					builder.AddAttribute(5, "BlockID", lot != null ? lot.BlockID : null);
					builder.AddComponentReferenceCapture(1, instance =>
                    {
                        // Validate that the instantiated internal component inherits the base component
                        if (instance is NbBaseComponent nbComponent)
                        {
                            nubeticoTab.InstanciaComponente = nbComponent;
                            nbComponent.EstadoControl = controlStatus;
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
            catch (Exception ex)
            {
                return;
            }
        }

		private void ShowInfoNotification()
		{
			var notification = new NotificationMessage
			{
				Severity = NotificationSeverity.Info,
				Summary = Localizer["Shared.Dialog.Atencion"],
				Detail = Localizer["Subdivision.Dialog.Selectrow"],
				Duration = 3500
			};

			notificationService.Notify(notification);
		}
	}
}
