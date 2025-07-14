using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProveedoresFacturas
{
    public class Entidad_Simplificado
    {
        public int ID_Entidad { get; set; }
        public string RFC { get; set; }
        public int? Dias_Credito { get; set; }
        public string? RazonSocial { get; set; }
    }
}
