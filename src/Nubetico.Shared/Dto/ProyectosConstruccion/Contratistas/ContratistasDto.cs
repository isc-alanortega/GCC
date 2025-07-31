using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Contratistas
{
    public class ContratistasDto
    {
        public int IdContratista { get; set; }
        public int IdEntidad { get; set; }
        public Guid UUID { get; set; }
        public string Folio { get; set; }
        public string Rfc { get; set; }
        public string Email { get; set; }
        public string Web { get; set; } = string.Empty;
        public bool Credito { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; } = string.Empty;
        public decimal LimiteCreditoMXN { get; set; }
        public decimal LimiteCreditoUSD { get; set; }
        public int DiasCredito { get; set; }
        public int DiasGracia { get; set; }
        public decimal SaldoMXN { get; set; }
        public decimal   SaldoUSD { get; set; }
        public int IdTipoPersonaSat { get; set; }
        public string CuentaContable { get; set; } = string.Empty;
        public int IdFormaPago { get; set; }
        public int IdTipoRegimenFiscal { get; set; }
        public bool Habilitado { get; set; }
        public DateTime FechaAlta { get; set; }
        public string NombreComercial { get; set; }
        public string RazonSocial { get; set; }
        public int IdRegimenFiscal { get; set; }
        public int IdTipoMetodoPago { get; set; }
        public int IdUsoCFDI { get; set; }
        public string ReferenciaBancaria { get; set; } = string.Empty;
        public int IdUsuarioAlta { get; set; }
    }
}
