using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.Core.Folios
{
    public class FolioResultSet
    {
        /// <summary>
        /// Prefijo del folio, puede ser nulo
        /// </summary>
        public string Serie { get; set; } = string.Empty;
        /// <summary>
        /// Número de dígitos, puede ser nulo
        /// </summary>
        public int Digitos { get; set; }
        /// <summary>
        /// Número de folio a utilizar
        /// </summary>
        public int Folio { get; set; }
    }
}
