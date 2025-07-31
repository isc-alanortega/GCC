using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Supplies
{
    public class SuppliesDto
    {
        public int? SuppliesId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? TypeId { get; set; }
        public bool? Active { get; set; }
        public string Unit { get; set; }   
        public Guid? ActionUserGuid { get; set; }
    }
}
