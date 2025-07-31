using Radzen;
using Radzen.Blazor;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Components.Shared;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class SubdivisionsCatComponent : NbBaseComponent
    {
		RadzenDataGrid<SubdivisionsDto>? Grid { get; set; }
        private IEnumerable<SubdivisionsDto> SubdivisionsList { get; set; }
		private IList<SubdivisionsDto> SelectedSubdivision { get; set; } = new List<SubdivisionsDto>();
        private int Count { get; set; }
		private bool IsLoading { get; set; } = false;
		private string? FilterName { get; set; }

		protected override async Task OnInitializedAsync()
		{
			await base .OnInitializedAsync();

			TriggerMenuUpdate();
		}

		public async Task LoadData(LoadDataArgs args)
		{
			IsLoading = true;
			var result = await subdivisionService.GetPaginatedSubdivisions(args.Top ?? 0, args.Skip ?? 0, FilterName);
			if (!result.Success || result.Data == null)
			{
				IsLoading = false;
				StateHasChanged();

				return;
			}

			var paginatedList = JsonConvert.DeserializeObject<PaginatedListDto<SubdivisionsDto>>(result.Data.ToString());
			if (paginatedList != null)
			{
				SubdivisionsList = paginatedList.Data;
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

		private void OnValueChange(string? value)
		{
			FilterName = value;
		}

		private void OnClickOpen()
		{
			if (!SelectedSubdivision.Any())
			{
				ShowInfoNotification();
				return;
			}

			OpenDetailControl(TipoEstadoControl.Lectura, IconoBase, SelectedSubdivision.First());
		}

		private void OnClickAdd()
		{
            OpenDetailControl(TipoEstadoControl.Alta, "add");
        }

		private void OnClickEdit()
		{
			if (!SelectedSubdivision.Any())
			{
				ShowInfoNotification();
				return;
			}

			OpenDetailControl(TipoEstadoControl.Edicion, "edit_square", SelectedSubdivision.First());
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

		private async void OnClickFilter()
		{
			var args = new LoadDataArgs { Top = 20, Skip = 0 };
			await LoadData(args);
		}

		public void DataGridRowDoubleClick(DataGridRowMouseEventArgs<SubdivisionsDto> subdivision)
		{
			if (subdivision.Data == null)
			{
				return;
			}

			OpenDetailControl(TipoEstadoControl.Lectura, IconoBase, subdivision.Data);
		}

		private void OpenDetailControl(TipoEstadoControl controlStatus, string iconTab, SubdivisionsDto? subdivision = null)
		{
			try
			{
                // Create the new tab
                var nubeticoTab = new TabNubetico
                {
                    EstadoControl = controlStatus,
                    Icono = iconTab,
                    Text = controlStatus == TipoEstadoControl.Alta
                    ? $"{Localizer["Shared.Textos.Nuevo"]} {Localizer["Subdivisions.Text.Subdivision"]}"
                    : $"{Localizer["Subdivisions.Text.Subdivision"]} [{subdivision.Subdivision}]",
                    TipoControl = typeof(SubdivisionsDetComponent),
                    Repetir = true
                };

                // Instantiate the component in the tab
                string? subdivisionGuid = subdivision != null ? subdivision.UUID.ToString() : null;
                nubeticoTab.Componente = builder =>
                {
                    builder.OpenComponent(0, nubeticoTab.TipoControl);
                    builder.AddAttribute(1, "SubdivisionGuid", subdivisionGuid);
                    builder.AddAttribute(2, "ParentCatalog", this);
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
				Detail = Localizer["Subdivisions.Dialog.Selectrow"],
				Duration = 3500
			};

			notificationService.Notify(notification);
		}
	}
}
