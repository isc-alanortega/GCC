using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.DAL.ResultSets.Core
{
    public class ProveedorResultSet
    {
        /// <summary>
        /// Indica si la operación fue exitosa
        /// </summary>
        public bool bResult { get; set; }

        /// <summary>
        /// Mensaje de resultado o error
        /// </summary>
        public string vchMessage { get; set; } = string.Empty;

        /// <summary>
        /// ID del proveedor creado (si aplica)
        /// </summary>
        public int IdProveedor { get; set; }
    }
}
