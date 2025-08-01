﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Palmaterra.DAL.Models;

public partial class Almacenes_Movimientos
{
    public int IDMovimiento { get; set; }

    public string IDAlmacen { get; set; }

    public string IDAlmacenDestino { get; set; }

    public DateTime Fecha { get; set; }

    public string Tipo { get; set; }

    public int Subtipo { get; set; }

    public string Referencia { get; set; }

    public bool? EsTraspasoOrigen { get; set; }

    public bool? EsTraspasoDestino { get; set; }

    public int? MovimientoTraspasoOrigen { get; set; }

    public int? MovimientoTraspasoDestino { get; set; }

    public string Observaciones { get; set; }

    public int? IDSucursal { get; set; }

    public string TipoDocumento { get; set; }

    public int? IDDocumento { get; set; }

    public string Status { get; set; }

    public int? IDUsuarioAlta { get; set; }

    public int? IDResponsable { get; set; }

    public int? IDObra { get; set; }

    public virtual ICollection<Almacenes_Movimientos_Partidas> Almacenes_Movimientos_Partidas { get; set; } = new List<Almacenes_Movimientos_Partidas>();
}