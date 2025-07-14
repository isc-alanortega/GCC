using Nubetico.WebAPI.Application.Modules.Core.Constans;
using Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices.Interfaces;
using Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices.SendInvoice;
using Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices.Validator;
using System.Diagnostics;

namespace Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices
{
    /// <summary>
    /// Factory class that creates and returns the appropriate Invoice services 
    /// based on the tenant name passed as a parameter.
    /// Implements the IInvoiceServiceFactory interface.
    /// </summary>
    /// <param name="serviceProvider">
    ///  Initializes a new instance of the <see cref="InvoiceServiceFactory"/> class.
    ///  The <see cref="IServiceProvider"/> used to resolve services.
    /// </param>
    public class InvoiceServiceFactory(IServiceProvider serviceProvider,
        MoregarInvoiceValidator moregarInvoiceValidator,
        MoregarSendInvoice moregarSendInvoice
    ) : IInvoiceServiceFactory
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;


        private readonly IValidateXml _moregarInvoiceValidator = moregarInvoiceValidator;
        private readonly ISendInvoice _moregarSendInvoice = moregarSendInvoice;

        /// <summary>
        /// Returns a tuple containing the appropriate validator and sender services 
        /// based on the provided tenant name.
        /// </summary>
        /// <param name="tenant">The tenant name that determines which services to return.</param>
        /// <returns>
        /// A tuple of <see cref="IValidateXml"/> and <see cref="ISendInvoice"/> services 
        /// for the corresponding tenant.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown if the tenant name is not recognized.</exception>
        public (IValidateXml? validator, ISendInvoice? sender) GetServicesForTenant(string tenant) => tenant switch //Switch on the tenant name to determine which services to return
            {
                /// If the tenant is CLIENTE_DEV, return the services needed for testing 
                /// invoices locally, likely on localhost or in a development environment.
                TenantsNames.CLIENTE_DEV => ((IValidateXml validator, ISendInvoice sender))(
                                        _moregarInvoiceValidator, _moregarSendInvoice
                                    ),
                TenantsNames.MOREGAR => ((IValidateXml validator, ISendInvoice sender))(
                                        _moregarInvoiceValidator, _moregarSendInvoice
                                    ),
                // Default case:
                _ => (null, null),
            };
    }
}
