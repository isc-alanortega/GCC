using Nubetico.Shared.Dto.Core;

namespace Nubetico.Shared.Dto.ProveedoresFacturas
{
    public class UploadInvoiceRequestDto
    {
        public Entidad_Simplificado? ProviderData { get; set; }
        public XmlElementsDto Invoice { get; set; }
    }
}
