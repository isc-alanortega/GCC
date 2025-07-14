using System.Xml.Linq;
using Microsoft.Extensions.Localization;
using Nubetico.Frontend.Services.Core.XmlServices.Mapper;
using Nubetico.Frontend.Services.Core.XmlServices.ReadXmlServices;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.Common;

namespace Nubetico.Frontend.Services.Core.XmlServices
{
    /// <summary>
    /// The InvoiceXMLExtractorService class is responsible for extracting data from an XML invoice document.
    /// It utilizes an XML reader to parse the document and a data mapper to map the extracted data 
    /// to a structured object.
    /// </summary>
    public class InvoiceXMLExtractorService
    {
        private readonly IXmlReader _xmlReader;
        private readonly IFacturaDataMapper _facturaDataMapper;
        private readonly IStringLocalizer<SharedResources>? _localizer;

        /// <summary>
        /// Gets the extracted invoice data.
        /// </summary>
        public XmlElementsDto InvoiceData { get; private set; }

        private string? RfcProvider { get; set; }

        /// <summary>
        /// Initializes a new instance of the InvoiceXMLExtractorService class.
        /// </summary>
        /// <param name="xmlReader">An implementation of IXmlReader for reading XML documents.</param>
        /// <param name="facturaDataMapper">An implementation of IFacturaDataMapper for mapping XML data to objects.</param>
        public InvoiceXMLExtractorService(
            IXmlReader xmlReader,
            IFacturaDataMapper facturaDataMapper,
            IStringLocalizer<SharedResources>? localizer)
        {
            _xmlReader = xmlReader;
            _facturaDataMapper = facturaDataMapper;
            _localizer = localizer;
            InvoiceData = new XmlElementsDto();
        }

        /// <summary>
        /// Asynchronously reads the XML document from the provided stream and maps the data to DatosFactura.
        /// </summary>
        /// <param name="fileStream">The stream containing the XML document to be read.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        public async Task<ResponseDto<object>> ReadDocumentAsync(Stream fileStream)
        {
            try
            {
                var xmlDocument = await _xmlReader.ReadXmlAsync(fileStream);
                string? nameSpace = GetNamesSpaces(xmlDocument);
                if (nameSpace is null)
                    return new ResponseDto<object>(false, "Error al leer el xml");

                _facturaDataMapper.Mapear(xmlDocument, nameSpace, InvoiceData);
            }
            catch (Exception ex)
            {
                return new ResponseDto<object>(false, "Error desconocido", ex);
            }

            return new ResponseDto<object>(success: true);
        }

        /// <summary>
        /// Refresh DatosFactura to Initial
        /// </summary>
        public void CleanData() => InvoiceData = new XmlElementsDto();

        /// <summary>
        /// Extracts the namespace from the provided XDocument.
        /// </summary>
        /// <param name="xmlDocument">The XDocument from which to extract the namespace.</param>
        /// <returns>The extracted namespace as a string.</returns>
        private string? GetNamesSpaces(XDocument xmlDocument)
        {
            var namespaces = xmlDocument?.Root?.Attributes()
                .Where(attr => attr.IsNamespaceDeclaration)
                .ToDictionary(attr => attr.Name.LocalName, attr => attr.Value);

            return namespaces != null && namespaces!.TryGetValue("cfdi", out string? cfdiNamespace) ? cfdiNamespace : null;
        }

        public string GetNameTypeInvoice() => InvoiceData?.TipoDeComprobante?.ToUpper() switch
        {
            "I" => _localizer["Invoice.Type.Income"],
            "E" => _localizer["Invoice.Type.Expense"],
            "T" => _localizer["Invoice.Type.Transfer"],
            "P" => _localizer["Invoice.Type.Payment"],
            "N" => _localizer["Invoice.Type.Payroll"],
            _ => string.Empty,
        };
    }
}
