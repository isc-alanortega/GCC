using Azure;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProveedoresFacturas;

namespace Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices.Interfaces
{
    public interface ISendInvoice
    {
        public Task<ResponseDto<object>> SendInvoiceAsync(XmlElementsDto invoice, Entidad_Simplificado providerData);
    }
}
