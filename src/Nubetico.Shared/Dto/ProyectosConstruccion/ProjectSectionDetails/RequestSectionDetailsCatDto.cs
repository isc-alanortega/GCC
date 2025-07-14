using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.ProjectSectionDetails
{
    public class RequestSectionDetailsCatDto
    {
        public int Limit { get; set; }
        public int OffSet { get; set; }
        public string? OrderBy { get; set; }
        public string? Property { get; set; }

        public string? Project { get; set; }
        public string? Section { get; set; }
    }
}
