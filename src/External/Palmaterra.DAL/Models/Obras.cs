﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Palmaterra.DAL.Models;

public partial class Obras
{
    public int IDObra { get; set; }

    public string Nombre { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public string Observaciones { get; set; }

    public int? IDEstado { get; set; }

    public int? IDCiudad { get; set; }

    public string Colonia { get; set; }

    public string Calle { get; set; }

    public string NoInterior { get; set; }

    public string NoExterior { get; set; }

    public int? IDClasificacion { get; set; }

    public int? IDResidente { get; set; }

    public int? IDTipo { get; set; }

    public bool? Completada { get; set; }

    public bool? EsProyectoObra { get; set; }

    public int? IDCoordinador { get; set; }

    public int? IDCategoria { get; set; }

    public int? IDCoordDestajo { get; set; }

    public int? IDSegmento { get; set; }

    public int? IDTipoObra { get; set; }
}