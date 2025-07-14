using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion
{
    public class ProveedoresDto
    {
        public Guid? UUID { get; set; }
        public string Folio { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int IdEstadoProveedor { get; set; }
        public string EstadoProveedor { get; set; } = string.Empty;
        public string RFC { get; set; } = string.Empty;
        public string CreditoPesos { get; set; } = string.Empty;
        public string CreditoDolares { get; set; } = string.Empty;
        public string RegimenFiscal { get; set; } = string.Empty;
        public bool TieneCredito { get; set; }
        public string DiasCredito { get; set; } = string.Empty;
        public string DiasGracia { get; set; } = string.Empty;
        public string SaldoPesos {get; set; } = string.Empty;
        public string SaldoDolares { get; set; } = string.Empty;
        public string Web { get; set; } = string.Empty ;
        public string Correo { get; set; }= string.Empty;
        public string CuentaContable { get; set; } = string.Empty;
        public string FormaPago { get; set; } = string.Empty;
    }
}
