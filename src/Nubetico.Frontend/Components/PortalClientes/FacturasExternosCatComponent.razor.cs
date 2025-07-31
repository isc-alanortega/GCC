using Radzen;
using Radzen.Blazor;
using Nubetico.Shared.Dto.PortalClientes;
using Nubetico.Frontend.Components.Dialogs.PortalClientes;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

namespace Nubetico.Frontend.Components.PortalClientes
{
    public partial class FacturasExternosCatComponent
    {
        private ExternalInvoicesFilter Filter { get; set; } = new ExternalInvoicesFilter();
        private RadzenDataGrid<ExternalClientInvoices>? InvoicesGrid { get; set; }
        private List<ExternalClientInvoices>? Invoices { get; set; }
        private int Count { get; set; }
        private bool IsLoading { get; set; } = false;
        private IList<ExternalClientInvoices> SelectedInvoices { get; set; } = new List<ExternalClientInvoices>();
        private decimal TotalSum;
        private decimal BalanceSum;

        #region Funciones
        protected override async Task OnInitializedAsync()
        {
            Filter = new ExternalInvoicesFilter
            {
                DateFrom = DateTime.Today.AddDays(-7),
                DateTo = DateTime.Today.AddDays(7),
                Type = "TODAS",
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
                    TotalSum = Invoices.Sum(invoice => invoice.Total);
                    BalanceSum = Invoices.Sum(invoice => invoice.Balance ?? 0);
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

        private async Task DataGridRowDoubleClick(DataGridRowMouseEventArgs<ExternalClientInvoices> args)
        {
			var selectedInvoice = args.Data;

			var pdfBytes = await documentsService.DownloadInvoicePDF(selectedInvoice);
			var base64PDF = Convert.ToBase64String(pdfBytes);
			var dataUrl = $"data:application/pdf;base64,{base64PDF}";

            DialogService.Open<ShowInvoiceDialogComponent>("Visualizador PDF",
                new Dictionary<string, object>()
                {
                    { "InvoiceUrl", dataUrl },
                    { "Invoice", selectedInvoice }
                },
                new DialogOptions()
                {
                    Width = "850px",
                    Height = "700px",
                    Resizable = true,
                    Draggable = true
                });
        }

        private async void DownloadInvoiceZIP(ExternalClientInvoices invoice)
        {
            try
            {
                var zipBytes = await documentsService.DownloadInvoiceZip(invoice);
                var fileName = $"Factura_{invoice.Folio}.zip";

                using var streamRef = new DotNetStreamReference(new MemoryStream(zipBytes));
                await JS.InvokeVoidAsync("downloadHelper", fileName, streamRef);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private BadgeStyle GetBadgeColor(string estatus) => estatus switch
        {
            "PAGADA" => BadgeStyle.Info,
            "ACTIVA" => BadgeStyle.Success,
            "CANCELADA" => BadgeStyle.Warning,
            _ => BadgeStyle.Light,
        };

        private void ShowTooltip(ElementReference elementReference, TooltipOptions options = null) => tooltipService.Open(elementReference, "Descargar ZIP", options);
        
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

        private static List<string> TypeList()
        {
            return
            [
                "TODAS",
                "FACTURA",
                "PAGO",
                "NOTA DE CRÉDITO"
            ];
        }
        #endregion
    }
}
