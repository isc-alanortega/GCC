using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.ProjectSectionDetails
{
    public class SectionDetailsDto
    {
        public Guid? ProjectGuid { get; set; }

        public Guid SectionGuid { get; set; }

        public string Project { get; set; }

        public string Section { get; set; }

        public string Description { get; set; }

        public string? ProjectedStartDate { get; set; }

        public string? ProjectedEndDate { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        public string GeneralConractor { get; set; }

        public bool? Complete { get; set; }
    }
}
