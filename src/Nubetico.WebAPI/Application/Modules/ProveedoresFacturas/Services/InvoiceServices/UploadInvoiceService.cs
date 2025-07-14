using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProveedoresFacturas;
using Microsoft.Extensions.Localization;
using Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices.Interfaces;

namespace Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices
{
    /// <summary>
    /// Service for handling the validation and sending of invoices for a specific tenant.
    /// This service interacts with the database to retrieve configuration settings (tenant details),
    /// validates the invoice using the appropriate validation rules for the tenant, and sends the invoice to the provider.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The service is designed to work with an external invoice validation service and a provider-specific invoice sender.
    /// It retrieves tenant-specific information (e.g., configuration or rules) and uses that information to determine
    /// how the invoice should be validated and sent.
    /// </para>
    /// <para>
    /// The tenant is determined dynamically, and the service performs asynchronous operations
    /// to validate the invoice, handle errors, and send the invoice if all validations pass.
    /// </para>
    /// </remarks>
    public class UploadInvoiceService(
        IDbContextFactory<CoreDbContext> coreDbContextFactory,
        IStringLocalizer<SharedResources> localizer,
        IInvoiceServiceFactory invoiceServiceFactory
    )
    {
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory = coreDbContextFactory;
        private readonly IStringLocalizer<SharedResources> _localizer = localizer;
        private readonly IInvoiceServiceFactory _invoiceServiceFactory = invoiceServiceFactory;

        /// <summary>
        /// Retrieves the tenant value for the given parameter ID (5) from the database.
        /// </summary>
        /// <returns>
        /// A string representing the tenant value. Returns an empty string if no value is found.
        /// </returns>
        private async Task<string> GetTenant()
        {
            using var coreDbContext = _coreDbContextFactory.CreateDbContext();
            var tenant = await coreDbContext.Parametros.FirstOrDefaultAsync(item => item.IdParametro == 5);
            return tenant?.Valor1 ?? string.Empty;
        }

        /// <summary>
        /// Validates and sends an invoice for the provided data.
        /// </summary>
        /// <param name="invoice">
        /// The invoice data to be validated and sent.
        /// </param>
        /// <param name="providerData">
        /// The provider/issuer.
        /// </param>
        /// <returns>
        /// A ResponseDto object containing the status and message of the invoice processing result.
        /// </returns>
        public async Task<ResponseDto<object>> SendInvoice(XmlElementsDto invoice, Entidad_Simplificado providerData)
        {
            // Get the tenant name
            string tenat = await GetTenant();

            // Retrieve the validator and sender functions
            try
            {
                var (validator, sender) = invoiceServiceFactory.GetServicesForTenant(tenat);

                // Validate the invoice and handle errors
                var (error, complement) = await validator.ValidateInvoiceAsync(invoice, providerData.RFC);
                if (!string.IsNullOrEmpty(error))
                {
                    var errorMessage = _localizer[error];
                    return new(false, complement != null ? string.Format(errorMessage, complement) : errorMessage);
                }

                // Send the invoice if validation is successful
                var responseSender = await sender.SendInvoiceAsync(invoice, providerData);
                return responseSender;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new ResponseDto<object>();
            }
        }
    }
}