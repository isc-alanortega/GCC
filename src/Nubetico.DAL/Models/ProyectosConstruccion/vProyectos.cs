﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Nubetico.DAL.Models.ProyectosConstruccion;

public partial class vProyectos
{
    public string Proyecto { get; set; }

    public DateTime Fecha_Alta { get; set; }

    public DateTime? Fecha_Inicio_Proyectada { get; set; }

    public DateTime? Fecha_Terminacion_Proyectada { get; set; }

    public string Fraccionamiento { get; set; }

    public int? Id_Fraccionamiento { get; set; }

    public bool Habilitado { get; set; }

    public string Unidad_Negocio { get; set; }

    public int Id_Unidad_Negocio { get; set; }

    public string TipoProyecto { get; set; }

    public string Folio { get; set; }

    public Guid? ProyectoGuid { get; set; }

    public string Encargado { get; set; }

    public string Estatus { get; set; }

    public int IdEstatus { get; set; }
}