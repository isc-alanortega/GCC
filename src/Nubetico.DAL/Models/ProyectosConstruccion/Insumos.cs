﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Nubetico.DAL.Models.ProyectosConstruccion;

public partial class Insumos
{
    public int ID { get; set; }

    public string Codigo { get; set; }

    public string Descripcion { get; set; }

    public int Id_Tipo { get; set; }

    public DateTime Fecha_Alta { get; set; }

    public int Id_Usuario_Alta { get; set; }

    public DateTime? Fecha_Modificacion { get; set; }

    public int? Id_Usuario_Modificacion { get; set; }

    public bool Habilitado { get; set; }

    public virtual ICollection<Relacion_Precios_Unitarios_Insumos> Relacion_Precios_Unitarios_Insumos { get; set; } = new List<Relacion_Precios_Unitarios_Insumos>();
}