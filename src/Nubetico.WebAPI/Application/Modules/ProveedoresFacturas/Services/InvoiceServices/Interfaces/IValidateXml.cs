using Nubetico.Shared.Dto.Core;

namespace Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices.Interfaces
{
    public interface IValidateXml
    {
        public Task<(string error, string? complement)> ValidateInvoiceAsync(XmlElementsDto invoice, string rfcEmisor);
    }
}
