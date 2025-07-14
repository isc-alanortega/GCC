using Radzen;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Nubetico.Shared.Dto.ProveedoresFacturas;
using Nubetico.Frontend.Services.Core.XmlServices;
using Nubetico.Frontend.Services.ProveedoresFacturas;
using Microsoft.VisualBasic;

namespace Nubetico.Frontend.Components.Dialogs
{
    public partial class UploadXmlDialogComponet
    {
        #region INJECTIONS
        /// <summary>
        /// Service to extract data from XML invoices.
        /// </summary>
        [Inject] private InvoiceXMLExtractorService? XmlService {  get; set; }

        /// <summary>
        /// Service to manage dialog interactions.
        /// </summary>
        [Inject] private DialogService? DialogService { get; set; }

        /// <summary>
        /// Service to manage notifications.
        /// </summary>
        [Inject] private NotificationService? NotificationService { get; set; }

        /// <summary>
        /// Service to handle invoice operations.
        /// </summary>
        [Inject] private FacturasService? FacturasService { get; set; }

        /// <summary>
        /// Localizer for internationalization.
        /// </summary>
        [Inject] private IStringLocalizer<SharedResources>? Localizer { get; set; }

        #endregion

        #region PARAMETERS
        /// <summary>
        /// The basic information (ID, RFC, and business name) of the user uploading the document.
        /// </summary>
        [Parameter] public Entidad_Simplificado? ProviderData {  get; set; }
        #endregion

        #region PROPERTYS
        /// <summary>
        /// Indicates the maximum allowed file size (20 MB)
        /// </summary>
        private const int MAX_FILE_SIZE = 20 * 1024 * 1024;

        /// <summary>
        /// Indicates that the XML is being read
        /// </summary>
        private bool IsReadingXml { get; set; } = false;

        /// <summary>
        /// Indicates if the XML data is ready
        /// </summary>
        private bool IsReadyXmlData { get; set; } = false;

        /// <summary>
        /// Indicates that a request is being made to the API to send the invoice
        /// </summary>
        private bool IsSeadingInvoice { get; set; } = false;

        /// <summary>
        /// Invoice date
        /// </summary>
        private DateTime? XmlDate { get; set; }

        /// <summary>
        /// Credit days according to the invoice and the provider's credit days
        /// </summary>
        /// 
        private DateTime? ExpirationDate { get; set; }
        /// <summary>
        /// The subtotal amount parsed from the XML invoice.
        /// </summary>
        private decimal? XmlSubtotal { get; set; }

        /// <summary>
        /// The total amount parsed from the XML invoice.
        /// </summary>
        private decimal? XmlTotal { get; set; }

        /// <summary>
        /// The withholding tax amount parsed from the XML invoice.
        /// </summary>
        private decimal? XmlWithholding { get; set; }

        /// <summary>
        /// The transfer tax amount parsed from the XML invoice.
        /// </summary>
        private decimal? XmlTransfer { get; set; }

        /// <summary>
        /// The exchange rate parsed from the XML invoice.
        /// </summary>
        private decimal? XmlExchangeRate { get; set; }

        #endregion

        #region BLAZOR LIFE CICLE METHODS
        protected override void OnInitialized()
        {
            XmlService!.CleanData();
            base.OnInitialized();
        }
        #endregion

        #region METHODS ON CHANGE INVOICE
        /// <summary>
        /// Event handler when the invoice file is changed (uploaded).
        /// </summary>
        private async Task OnInvoiceChangeAsync(UploadChangeEventArgs args)
        {
            if (IsReadingXml) return;

            IsReadingXml = true;

            try
            {
                var file = args.Files.FirstOrDefault();
                if (file == null)
                {
                    HandleFileError();
                    return;
                }

               await ProcessFileAsync(file!);
            }
            catch (Exception ex)
            {
                NotifyAcces("Error desconocido", "Ocurrio un problema al intentar subir la factura.");
                HandleFileError();
                return;
            }

            IsReadingXml = false;
        }

        /// <summary>
        /// Handle errors when the file cannot be processed correctly.
        /// </summary>
        private void HandleFileError()
        {
            if (!IsReadyXmlData)
                NotifyAcces("Error al leer el documento", "No fue posible leer el documento.");

            ResetDialogState();
        }

        /// <summary>
        ///  Processes the uploaded file by reading and extracting data from it asynchronously.
        /// </summary>
        private async Task ProcessFileAsync(IBrowserFile file)
        {
            if (file == null) 
            {
                NotifyAcces("Ocurrio un problema", "No se puede leeer el docuemnto");
                return;
            }

            using var fileStream = file.OpenReadStream(MAX_FILE_SIZE); 
            var response = await XmlService!.ReadDocumentAsync(fileStream);

            if (!response.Success)
            {
                NotifyAcces("Ocurrio un problema", response.Message!);
                return;
            }

            // Data is ready for further processing.
            IsReadyXmlData = true; 
            ParseInvoiceDate();
            StateHasChanged();
        }
        #endregion

        #region UTILS METHODS
        /// <summary>
        /// Resets the dialog state, clearing all fields.
        /// </summary
        private void ResetDialogState()
        {
            XmlService!.CleanData();
            XmlDate = null;
            ExpirationDate = null;
            XmlExchangeRate = null;
            XmlSubtotal = null;
            XmlTotal = null;
            XmlTransfer = null;
            XmlWithholding = null;
            IsReadyXmlData = false;
            IsReadingXml = false;
        }

        /// <summary>
        /// Show message.
        /// </summary>
        private void NotifyAcces(string summary, string details, NotificationSeverity severity = NotificationSeverity.Error)
        {
            NotificationService!.Notify(new NotificationMessage
            {
                Severity = severity,
                Summary = summary,
                Detail = details
            });
        }

        /// <summary>
        /// Parses the invoice date and related data (subtotal, total, etc.) from the XML document.
        /// </summary>
        private void ParseInvoiceDate()
        {
            if(!IsReadyXmlData) return;

            if (DateTime.TryParse(XmlService!.InvoiceData.Fecha, out var dateParsed))
            {
                XmlDate = dateParsed;
                ExpirationDate = dateParsed.AddDays(ProviderData?.Dias_Credito ?? 0);
            }

            XmlSubtotal = decimal.TryParse(XmlService!.InvoiceData.SubTotal, out decimal subTotal) ? subTotal : null;
            XmlTotal = decimal.TryParse(XmlService!.InvoiceData.Total, out decimal total) ? total : null;
            XmlTransfer = decimal.TryParse(XmlService!.InvoiceData.Traslado, out decimal transfer) ? transfer : null;
            XmlWithholding = decimal.TryParse(XmlService!.InvoiceData.SubTotal, out decimal withholding) ? withholding : null;
        }
        #endregion

        #region UPLOAD METHODS
        private async Task OnUploadInvoiceAsync()
        {
            if(IsSeadingInvoice) return;

            IsSeadingInvoice = true;

            var result = await FacturasService!.PostSendInvoiceProvider(new()
            {
                Invoice = XmlService!.InvoiceData,
                ProviderData = ProviderData
            });

            if (result != null && result.Success)
            {
                NotifyAcces("Cfdi subido con exito", "Se subio correctamente el cfdi", NotificationSeverity.Success);
            }
            else
            {
                NotifyAcces("Ocurrio un problema al intentar guardar el cfdi", result?.Message ?? "Error desconocido");
            }

            ResetDialogState();
            IsSeadingInvoice = false;
            DialogService!.Close(result?.Success ?? false);
        }
        #endregion
    }
}
