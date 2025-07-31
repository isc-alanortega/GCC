using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores
{
    public class ProveedorSaveDto
    {

        public Guid UUID { get; set; }
        public string Folio { get; set; }
        public string Rfc { get; set; }
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public int IdTipoPersonaSat { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Celular {  get; set; }
        public string Web { get; set; }
        public bool Credito { get; set; }
        public decimal LimiteCreditoMXN { get; set; }
        public decimal LimiteCreditoUSD { get; set; }
        public int DiasCredito { get; set; }
        public int? DiasGracia { get; set; }
        public decimal SaldoMXN { get; set; }
        public decimal SaldoUSD { get; set; }
        public string CuentaContable { get; set; }
        public int? IdFormaPago { get; set; }
        public int IdTipoMetodoPago { get; set; }
        public int IdUsoCFDI { get; set; }
        public int IdTipoRegimenFiscal { get; set; }
        public int IdTipoProveedor { get; set; }
        public int IdUsuarioAlta { get; set; }
        public bool Habilitado { get; set; }
        public int IdRegimenFiscal { get; set; }
        public DateTime FechaAlta { get; set; } = DateTime.Now;

    }
}
