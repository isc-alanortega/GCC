using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProveedoresFacturas
{
    public class ApiFacturaPeticion
    {
        public int ID_Entidad { get; set; }
        public DateTime Fecha_Inicio { get; set; }
        public DateTime Fecha_Fin { get; set; }
        public int Estatus { get; set; }
    }
}
