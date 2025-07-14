using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto
{
    public class ProjectSectionDataDto
    {
        public int? ProjectId { get; set; }
        public Guid? SectionGuid { get; set; }
        public int? SectionId { get; set; }
        public string? Folio {  get; set; }
        public int? Status { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? GeneralContractor { get; set; }
        public DateTime? ProjectedStartDate { get; set; }
        public DateTime? ProjectedEndDate { get; set; }
        public DateTime? RealStartDate { get; set; }
        public DateTime? RealEndDate { get; set; }
        public int Sequence { get; set; }
        public Guid? UserActionGuid { get; set; }
        public int ModelId { get; set; }
        public bool Active { get; set; } = true;

        public List<ProjectSectionPhaseDto> ProjectSectionPhase { get; set; } = [];

        public List<SectionLotsGridDto> SectionLots { get; set; } = [];
    }
}
