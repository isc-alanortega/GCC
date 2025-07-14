using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion
{
    public class ModelSuppliesSimplify
    {
        public string Type { get; set; }
        public string Supply { get; set; }
        public string Unit { get; set; }
        public decimal Volume { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }
}
