using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.WebAPI.Application.Modules.Core.Constans;
using Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices.Interfaces;

namespace Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices
{
    /// <summary>
    /// Manages the retrieval of invoice validation and sending services for different tenants.
    /// This service is responsible for providing the correct validator and sender based on the tenant identifier.
    /// </summary>
    public class InvoiceServicesManager 
    {
        private readonly IInvoiceServiceFactory _invoiceServiceFactory;

        public InvoiceServicesManager(IInvoiceServiceFactory invoiceServiceFactory)
        {
            _invoiceServiceFactory = invoiceServiceFactory;
        }

        public ResponseDto<(IValidateXml validator, ISendInvoice sender)> GetValidatorSenderMethods(string tenant)
        {
            try
            {
                var services = _invoiceServiceFactory.GetServicesForTenant(tenant);
                return new ResponseDto<(IValidateXml, ISendInvoice)>(true, data: services);
            }
            catch (Exception ex)
            {
                return new ResponseDto<(IValidateXml, ISendInvoice)>(false, ex.Message);
            }
        }
    }
}
