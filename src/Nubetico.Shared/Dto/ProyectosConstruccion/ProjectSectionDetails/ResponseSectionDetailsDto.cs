using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.ProjectSectionDetails
{
    public class ResponseSectionDetailsDto
    {
        public SectionDetailsDto SectionDetails { get; set; }
        public List<SectionLotsDto> SectionLots { get; set; }
    }
}
