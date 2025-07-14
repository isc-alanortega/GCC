using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Models
{
    public class ModelGridRequestDto
    {
        public string OrderBy { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Name { get; set; }
        public string Folio { get; set; }
    }
}
