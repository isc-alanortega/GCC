using Radzen;
using Radzen.Blazor;
using Nubetico.Frontend.Components.Dialogs;
using Nubetico.Shared.Dto.PortalClientes;

namespace Nubetico.Frontend.Components.PortalClientes
{
    public partial class FacturasExternosCatComponent
    {
        private ExternalInvoicesFilter Filter { get; set; } = new ExternalInvoicesFilter();
        private RadzenDataGrid<ExternalClientInvoices>? InvoicesGrid { get; set; }
        private List<ExternalClientInvoices>? Invoices { get; set; }
        private int Count { get; set; }
        private bool IsLoading { get; set; } = false;
        public IList<ExternalClientInvoices> SelectedInvoices { get; set; } = new List<ExternalClientInvoices>();

        #region Funciones
        protected override async Task OnInitializedAsync()
        {
            Filter = new ExternalInvoicesFilter
            {
                DateFrom = DateTime.Today.AddDays(-7),
                DateTo = DateTime.Today.AddDays(7),
                Status = "TODAS"
            };

            await base.OnInitializedAsync();
        }

        private async Task LoadData(LoadDataArgs args)
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                var limit = args.Top ?? 20;
                var offset = args.Skip ?? 0;

                var result = await invoicesService.GetPagedInvoicesAsync(limit, offset, Filter);
                if (result != null && result.Success && result.Data != null)
                {
                    Invoices = result.Data!.Data;
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

        private async void OnValueChange()
        {
            await LoadData(new LoadDataArgs { Top = 20, Skip = 0 });
            StateHasChanged();
        }

        private BadgeStyle GetBadgeColor(string estatus) => estatus switch
        {
            "PAGADA" => BadgeStyle.Info,
            "ACTIVA" => BadgeStyle.Success,
            "CANCELADA" => BadgeStyle.Warning,
            _ => BadgeStyle.Light,
        };

        private async Task OpenUploadXmModal()
        {
            var result = await DialogService.OpenAsync<UploadXmlDialogComponet>(title: "Cargar nuevo CFDI",
            parameters: new Dictionary<string, object>() { { "ProviderData", null } },
            options: new DialogOptions()
            {
                CloseDialogOnOverlayClick = true,
                AutoFocusFirstElement = true,
                CloseDialogOnEsc = true,
                Width = "700px"
            });

            if (result != null && result)
            {
                var dataArg = new LoadDataArgs();
                await LoadData(dataArg);
                StateHasChanged();
            }
        }

        private static List<string> StatusList()
        {
            return
            [
                "TODAS",
                "PAGADA",
                "PENDIENTE",
                "CANCELADA"
            ];
        }
        #endregion
    }
}
