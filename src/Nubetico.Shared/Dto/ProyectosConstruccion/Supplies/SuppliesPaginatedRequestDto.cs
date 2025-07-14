using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Supplies
{
    public class SuppliesPaginatedRequestDto
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string OrderBy { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public int? TypeId { get; set; }
    }
}
