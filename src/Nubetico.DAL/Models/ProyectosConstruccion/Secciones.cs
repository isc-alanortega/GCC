﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Nubetico.DAL.Models.ProyectosConstruccion;

public partial class Secciones
{
    public int Id_Seccion { get; set; }

    public int Id_Proyecto { get; set; }

    public string Nombre { get; set; }

    public string Descripcion { get; set; }

    public int Id_Contratista { get; set; }

    public DateTime Fecha_Inicio_Proyectada { get; set; }

    public DateTime Fecha_Terminacion_Proyectada { get; set; }

    public DateTime Fecha_Alta { get; set; }

    public int Id_Usuario_Alta { get; set; }

    public DateTime? Fecha_Modificacion { get; set; }

    public int? Id_Usuario_Modificacion { get; set; }

    public int Id_Estado { get; set; }

    public DateTime? Fecha_Inicio { get; set; }

    public DateTime? Fecha_Terminacion { get; set; }

    public int Secuencia { get; set; }

    public Guid UUID { get; set; }

    public string Folio { get; set; }

    public bool? Habilitado { get; set; }

    public int? Id_Modelo { get; set; }

    public virtual Modelos Id_ModeloNavigation { get; set; }
}