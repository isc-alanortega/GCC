using Nubetico.Shared.Dto.Core;
using System.Xml.Linq;

namespace Nubetico.Frontend.Services.Core.XmlServices.Mapper
{
    public interface IFacturaDataMapper
    {
        void Mapear(XDocument xmlDocument, string espacioNombre, XmlElementsDto facturaData);
    }
}
