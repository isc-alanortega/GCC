using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto
{
    public class ProjectSectionPhaseDto
    {
        public string? Folio { get; set; }
        public int? PhaseId { get; set; }
        public int? SectionId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Sequence { get; set; }
        public bool Complete { get; set; } = false;
        public Guid? SectionGuidTemp { get; set; }
        public bool? Active { get; set; } = true;
        public int? ProjectId { get; set; }
        public string? ProjectName { get; set; }
    }
}
