using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Supplies
{
    public class SuppliesFetcherDto
    {
        public IEnumerable<GroupsCatalogDto> TypesSupplies { get; set; }
        public IEnumerable<GroupsCatalogDto> UmSupplies { get; set; }
    }
}
