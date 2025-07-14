using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.Shared.Dto.Core;
using Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices.Interfaces;

namespace Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices.Validator
{
    public class MoregarInvoiceValidator(IDbContextFactory<CoreDbContext> coreDbContextFactory) : IValidateXml
    {
        private readonly List<string> _validInvoiceTypes = ["I"];
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory = coreDbContextFactory;

        public async Task<(string error, string? complement)> ValidateInvoiceAsync(XmlElementsDto invoice, string rfcEmisor)
        {
            using var coreDbContext = _coreDbContextFactory.CreateDbContext();

            if (!invoice.RfcEmisor!.Equals(rfcEmisor))
                return ("ProveedoresFacturas.Error.InvalidRfcIssuer", null);

            if (!_validInvoiceTypes.Contains(invoice.TipoDeComprobante!.ToUpper()))
                return ("ProveedoresFacturas.Error.InvalidInvoiceType", string.Join(", ", _validInvoiceTypes));

            if (!await coreDbContext.Entidades.AnyAsync(item => item.Rfc == invoice.RfcReceptor))
                return ("ProveedoresFacturas.Error.InvalidRfcRecipient", null);

            return (string.Empty, null);
        }
    }
}
