using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProveedoresFacturas
{
    public class FacturaDto
    {
        public int IDMovimiento { get; set; }
        public int? IDCargoAbono { get; set; }
        public int? Secuencia { get; set; }
        public int? IDProveedor { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string IDCompra { get; set; }
        public short? IDTipoCargoAbono { get; set; }
        public string Tipo { get; set; }
        public short? IDFormaPago { get; set; }
        public string Referencia { get; set; }
        public string Numero { get; set; }
        public string Banco { get; set; }
        public float? Monto { get; set; }
        public float? Importe { get; set; }
        public float? Total { get; set; }
        public float? Restante { get; set; }
        public bool Pagado { get; set; }
        public string Observaciones { get; set; }
        public int? IDMoneda { get; set; }
        public string Moneda { get; set; }
        public float? Conversion { get; set; }
        public float? SubTotal { get; set; }
        public float? Impuesto1 { get; set; }
        public float? Impuesto2 { get; set; }
        public float? Retencion1 { get; set; }
        public float? Retencion2 { get; set; }
        public float? GranTotal { get; set; }
        public int? IDCosto { get; set; }
        public short? Estado { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public bool? CFDI { get; set; }
        public string UUID { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int? IDUsuarioAlta { get; set; }
        public int? IDUsuarioCancelacion { get; set; }
        public int? EstatusProceso { get; set; }
        public string EstadoLeyenda { get; set; }
    }
}
