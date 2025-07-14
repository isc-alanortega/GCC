using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProveedoresFacturas
{
    public class ApiRespuesta<T>
    {
        public string estatus { get; set; }
        public string mensaje { get; set; }
        public T resultado { get; set; }
    }
}
