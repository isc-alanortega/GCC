using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Sections
{
    public class ProjectSectionFetcherDto
    {
        public PaginatedListDto<SectionLotsGridDto> LotsList {  get; set; }

        public IEnumerable<ElementsDropdownForm> SectionStatusList { get; set; }

        public IEnumerable<ElementsDropdownForm> GeneralContractorsList { get; set; }
        public IEnumerable<ElementsDropdownForm> Models { get; set; }
    }
}
