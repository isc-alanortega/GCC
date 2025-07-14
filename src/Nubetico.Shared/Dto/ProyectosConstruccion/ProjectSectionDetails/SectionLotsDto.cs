using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.ProjectSectionDetails
{
    public class SectionLotsDto
    {
        public Guid? ProjectGuid { get; set; }

        public Guid SectionGuid { get; set; }

        public string Project { get; set; }

        public string Subdivision { get; set; }

        public string SubdivisionStep { get; set; }

        public string Stage { get; set; }

        public int Lots { get; set; }
    }
}
