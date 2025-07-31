using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores
{
    public class ProveedorResult
    {
        public bool bResult { get; set; }
        public string vchMessage { get; set; } = string.Empty;
        public int IdProveedor { get; set; }
        public int IdEntidad { get; set; }
        public Guid UUID { get; set; }
        public string Folio { get; set; } = string.Empty;
        public string Rfc { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
        public string NombreComercial { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Web { get; set; } = string.Empty;
        public bool Credito { get; set; }
        public string Telefono { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public decimal LimiteCreditoMXN { get; set; }
        public decimal LimiteCreditoUSD { get; set; }
        public int DiasCredito { get; set; }
        public int DiasGracia { get; set; }
        public decimal SaldoMXN { get; set; }
        public decimal SaldoUSD { get; set; }
        public string CuentaContable { get; set; } = string.Empty;
        public int IdFormaPago { get; set; }
        public int IdTipoMetodoPago { get; set; }
        public int IdUsoCFDI { get; set; }
        public int IdTipoRegimenFiscal { get; set; }
        public int IdTipoProveedor { get; set; }
        public bool Habilitado { get; set; }
        public int IdRegimenFiscal { get; set; }
        public DateTime FechaAlta { get; set; }
    }
}
