using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.ProveedoresFacturas;
using System.Xml.Linq;

namespace Nubetico.Frontend.Services.Core.XmlServices.Mapper
{
    /// <summary>
    /// The FacturaDataMapper class is responsible for mapping XML data from an XDocument 
    /// to an XmlElementsData object. It extracts relevant information from the XML structure 
    /// and populates the properties of the XmlElementsData instance.
    /// </summary>
    public class FacturaDataMapper : IFacturaDataMapper
    {
        /// <summary>
        /// Maps the XML data from the provided XDocument to the given XmlElementsData object.
        /// The method extracts data related to the invoice, issuer, recipient, taxes, 
        /// and digital stamp.
        /// </summary>
        /// <param name="xmlDocument">The XDocument containing the XML data.</param>
        /// <param name="espacioNombre">The namespace used in the XML elements.</param>
        /// <param name="facturaData">The XmlElementsData object to populate with the mapped data.</param>
        public void Mapear(XDocument xmlDocument, string espacioNombre, XmlElementsDto facturaData)
        {
            // Maps the elements of the invoice
            #region COMROBANTE
            var comprobante = xmlDocument.Descendants(XName.Get("Comprobante", espacioNombre)).FirstOrDefault();
            MapearAtributos(comprobante, facturaData, new Dictionary<string, Action<string>>
            {
                { "Fecha", item => facturaData.Fecha = item },
                { "Folio", item => facturaData.Folio = item },
                { "SubTotal", item => facturaData.SubTotal = item },
                { "Moneda", item => facturaData.Moneda = item },
                { "TipoCambio", item => facturaData.TipoCambio = item },
                { "TipoDeComprobante", item => facturaData.TipoDeComprobante = item },
                { "FormaPago", item => facturaData.FormaPago = item },
                { "MetodoPago", item => facturaData.MetodoPago = item },
                { "Total", item => facturaData.Total = item },
                { "Serie", item => facturaData.Serie = item }
            });
            #endregion

            // Maps attributes of the issuer
            #region EMISOR
            MapearAtributos(xmlDocument.Descendants(XName.Get("Emisor", espacioNombre)).FirstOrDefault(), facturaData, new Dictionary<string, Action<string>>
            {
                { "Nombre", item => facturaData.Emisor = item },
                { "Rfc", item => facturaData.RfcEmisor = item },
                { "RegimenFiscal", item => facturaData.RegimenFiscalEmisor = item }
            });
            #endregion

            // Maps attributes of the recipient
            #region RECEPTOR
            MapearAtributos(xmlDocument.Descendants(XName.Get("Receptor", espacioNombre)).FirstOrDefault(), facturaData, new Dictionary<string, Action<string>>
            {
                { "Nombre", item => facturaData.Receptor = item },
                { "Rfc", item => facturaData.RfcReceptor = item },
                { "RegimenFiscalReceptor", item => facturaData.RegimenFiscalReceptor = item }
            });
            #endregion

            var impuestos = comprobante.Element(XName.Get("Impuestos", espacioNombre));
            if (impuestos != null)
            {
                facturaData.Retencion = impuestos.Attribute("TotalImpuestosRetenidos")?.Value ?? "-";
                facturaData.Traslado = impuestos.Attribute("TotalImpuestosTrasladados")?.Value ?? "-";
            }

            // Searches for the "TimbreFiscalDigital" node within the "Complemento"
            #region TIMBRE FISCAL
            var timbreFiscal = comprobante?.Descendants(XName.Get("Complemento", espacioNombre))
                                        .SelectMany(c => c.Descendants(XName.Get("TimbreFiscalDigital", "http://www.sat.gob.mx/TimbreFiscalDigital")))
                                        .FirstOrDefault();
            if (timbreFiscal != null)
            {
                facturaData.UUID = timbreFiscal.Attribute("UUID")?.Value ?? "-";
            }
            #endregion
        }

        private void MapearAtributos(XElement elemento, XmlElementsDto facturaData, Dictionary<string, Action<string>> asignaciones)
        {
            if (elemento != null)
            {
                foreach (var asignacion in asignaciones)
                {
                    var valor = elemento.Attribute(asignacion.Key)?.Value ?? "-";
                    asignacion.Value(valor);
                }
            }
        }
    }
}