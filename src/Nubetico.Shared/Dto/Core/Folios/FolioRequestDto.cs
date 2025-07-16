using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.Core.Folios
{
    public class FolioRequestDto
    {
        public string Alias { get; set; } = string.Empty;
        public int? IdSucursal { get; set; }
    }
}
