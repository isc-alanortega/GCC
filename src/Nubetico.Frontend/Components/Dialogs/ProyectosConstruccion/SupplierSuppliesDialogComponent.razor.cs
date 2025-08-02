using Microsoft.AspNetCore.Components;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Supplies;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.Dialogs.ProyectosConstruccion
{
	public partial class SupplierSuppliesDialogComponent : NbBaseComponent
	{
		[Parameter]
		public string SupplierName { get; set; } = string.Empty;

		[Parameter]
		public IEnumerable<int> ExistingSuppliesIds { get; set; } = Enumerable.Empty<int>();

		[Inject]
		private SuppliesService SuppliesDA { get; set; }

		private RadzenDataGrid<InsumosDto>? GridSupplies;
		private bool IsLoading { get; set; } = false;
		private int Count { get; set; }
		private IEnumerable<InsumosDto>? SuppliesList { get; set; }
		private SuppliesPaginatedRequestDto RequestForm { get; set; } = new();

		private List<InsumosDto> SelectedSuppliesList { get; set; } = new();
		private IList<InsumosDto> SelectedSupplies { get; set; } = new List<InsumosDto>();
		private bool SelectAll { get; set; } = false;

		private bool HasSelectedSupplies => SelectedSuppliesList.Any();

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			RequestForm.Limit = 10;
			RequestForm.Offset = 0;
		}

		public async Task LoadData(LoadDataArgs args)
		{
			if (IsLoading) return;

			try
			{
				IsLoading = true;

				var limit = args.Top ?? 10;
				var offset = args.Skip ?? 0;
				var orderBy = args.Sorts != null && args.Sorts.Any()
					? string.Join(",", args.Sorts.Select(s => $"{s.Property} {(s.SortOrder == SortOrder.Descending ? "desc" : "asc")}"))
					: "Code asc";

				RequestForm.Limit = limit;
				RequestForm.Offset = offset;

				var result = await SuppliesDA.GetPaginatedSupplies(request: RequestForm);

				if (result != null && result.Success && result.Data != null)
				{
					// Filtrar insumos que no sean "MANO DE OBRA" y que no estén ya asignados al proveedor
					var availableSupplies = result.Data!.Data
						.Where(i => i.Type != "MANO DE OBRA" && !ExistingSuppliesIds.Contains(i.ID))
						.ToList();

					SuppliesList = availableSupplies;
					Count = availableSupplies.Count();
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Error al cargar datos: {ex.Message}");
				ShowErrorNotification("Error al cargar los insumos");
			}
			finally
			{
				IsLoading = false;
				StateHasChanged();
			}
		}

		private bool IsSupplySelected(InsumosDto supply)
		{
			return SelectedSuppliesList.Any(s => s.ID == supply.ID);
		}

		private void OnSupplySelectionChange(InsumosDto supply, bool isSelected)
		{
			if (isSelected)
			{
				if (!SelectedSuppliesList.Any(s => s.ID == supply.ID))
				{
					SelectedSuppliesList.Add(supply);
				}
			}
			else
			{
				SelectedSuppliesList.RemoveAll(s => s.ID == supply.ID);
			}

			// Update SelectAll state
			UpdateSelectAllState();
			StateHasChanged();
		}

		private void OnSelectAllChange(bool selectAll)
		{
			SelectAll = selectAll;

			if (selectAll)
			{
				// Add all visible supplies to selection
				if (SuppliesList != null)
				{
					foreach (var supply in SuppliesList)
					{
						if (!SelectedSuppliesList.Any(s => s.ID == supply.ID))
						{
							SelectedSuppliesList.Add(supply);
						}
					}
				}
			}
			else
			{
				// Remove all visible supplies from selection
				if (SuppliesList != null)
				{
					var visibleIds = SuppliesList.Select(s => s.ID).ToHashSet();
					SelectedSuppliesList.RemoveAll(s => visibleIds.Contains(s.ID));
				}
			}

			StateHasChanged();
		}

		private void UpdateSelectAllState()
		{
			if (SuppliesList != null && SuppliesList.Any())
			{
				SelectAll = SuppliesList.All(supply => SelectedSuppliesList.Any(s => s.ID == supply.ID));
			}
			else
			{
				SelectAll = false;
			}
		}

		private void OnClickAccept()
		{
			if (!SelectedSuppliesList.Any())
			{
				ShowErrorNotification("Debe seleccionar al menos un insumo");
				return;
			}

			dialogService.Close(SelectedSuppliesList);
		}

		private void OnClickCancel()
		{
			dialogService.Close();
		}

		private BadgeStyle GetBadgeStyle(int id_type) => id_type switch
		{
			2 => BadgeStyle.Info,
			3 => BadgeStyle.Warning,
			_ => BadgeStyle.Base,
		};
		private EventCallback<bool> OnSelectAllChangeCallback =>
	EventCallback.Factory.Create<bool>(this, OnSelectAllChange);

		private void ShowErrorNotification(string message)
		{
			var notification = new NotificationMessage
			{
				Severity = NotificationSeverity.Error,
				Summary = "Error",
				Detail = message,
				Duration = 3500
			};

			notificationService.Notify(notification);
		}
	}
}