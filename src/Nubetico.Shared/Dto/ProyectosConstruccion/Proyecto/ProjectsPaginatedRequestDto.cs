using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto
{
    public class ProjectsPaginatedRequestDto
    {
        public int Limit { get; set; }
        public int OffSet { get; set; }
        public string OrderBy { get; set; }
        public int? SubdivisionId { get; set; }
        public int? BusinessUnitId { get; set; }
        public string? Nombre { get; set; }
        public int? Estado { get; set; }
        public string? Encargado { get; set; }
    }
}
