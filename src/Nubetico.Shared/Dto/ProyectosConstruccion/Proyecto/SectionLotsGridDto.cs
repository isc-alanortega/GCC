using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto
{
    public class SectionLotsGridDto
    {
        public int? SectionLotsId {  get; set; }
        public int? LotId { get; set;  }
        public int? SectionId { get; set; }
        public int? ModelId { get; set; }
        public int? LotNumber { get; set; }
        public bool IsLotSelected { get; set; } = false;
    }
}
