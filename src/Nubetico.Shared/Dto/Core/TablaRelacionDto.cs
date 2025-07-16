using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.Core
{
    public class TablaRelacionDto
    {
        public int ID { get; set; }
        public int? Valor { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
    public class TablaRelacionStringDto
    {
        public int ID { get; set; }
        public string ValorString { get; set; } = "";
        public string Descripcion { get; set; } = "";
    }
}
