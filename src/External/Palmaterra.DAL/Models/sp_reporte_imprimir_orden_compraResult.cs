﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Palmaterra.DAL.Models
{
    public partial class sp_reporte_imprimir_orden_compraResult
    {
        [StringLength(15)]
        public string OrdenCompra { get; set; }
        [StringLength(2500)]
        public string Observaciones { get; set; }
        [StringLength(30)]
        public string FechaOrden { get; set; }
        [StringLength(152)]
        public string Solicitante { get; set; }
        [StringLength(1000)]
        public string Proveedor { get; set; }
        [StringLength(250)]
        public string Obra { get; set; }
        [StringLength(250)]
        public string Producto { get; set; }
        [Column("Cantidad", TypeName = "decimal(18,2)")]
        public decimal Cantidad { get; set; }
        [StringLength(250)]
        public string Embalaje { get; set; }
    }
}
