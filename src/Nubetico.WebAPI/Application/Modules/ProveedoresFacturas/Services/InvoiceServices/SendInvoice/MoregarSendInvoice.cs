using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProveedoresFacturas;
using Nubetico.WebAPI.Application.External.ProveedoresFacturas;
using Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices.Interfaces;

namespace Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices.SendInvoice
{
    public class MoregarSendInvoice : ISendInvoice
    {
        private object GetRequest(XmlElementsDto invoice, Entidad_Simplificado providerData)
        {
            string referencia = $"{(invoice!.Serie == "-" ? string.Empty : invoice.Serie)}{(invoice.Folio == "-" ? string.Empty : invoice.Folio)}";

            DateTime dateParsed;
            DateTime? expirationDate = null;
            if (DateTime.TryParse(invoice.Fecha, out dateParsed))
            {
                expirationDate = dateParsed.AddDays(providerData?.Dias_Credito ?? 0);
            }

            string? total = invoice.Total != "-" ? invoice.Total : null;

            return new
            {
                Secuencia = 1,
                IDCargoAbono = -1,
                IDProveedor = providerData?.ID_Entidad,
                Fecha = dateParsed,
                FechaVencimiento = expirationDate ?? DateTime.Now,
                IDCompra = referencia,
                IDTipoCargoAbono = 410,
                Referencia = referencia,
                Tipo = "C",
                Monto = total,
                Importe = total,
                Total = total,
                Restante = invoice.Total,
                Pagado = false,
                Estado = 1,
                IDMoneda = invoice!.Moneda == "MXN" ? 1 : 2,
                Conversion = invoice.TipoCambio,
                SubTotal = invoice.SubTotal != "-" ? invoice.SubTotal : null,
                Impuesto1 = invoice.Traslado != "-" ? invoice.Traslado : null,
                Retencion1 = invoice.Retencion != "-" ? invoice.Retencion : null,
                GranTotal = invoice.Total,
                CFDI = true,
                invoice!.UUID,
                FechaAlta = DateTime.Now,
                IDUsuarioAlta = providerData?.ID_Entidad,
                EstatusProceso = 1
            };
        }

        public async Task<ResponseDto<object>> SendInvoiceAsync(XmlElementsDto invoice, Entidad_Simplificado providerData)
        {
            var result = await ClientesEndpoints.SendInvoiceProvider(GetRequest(invoice, providerData));
            return result;
        }
    }
}
