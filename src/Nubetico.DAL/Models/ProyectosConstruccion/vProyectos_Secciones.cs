﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Nubetico.DAL.Models.ProyectosConstruccion;

public partial class vProyectos_Secciones
{
    public Guid? ProyectoGuid { get; set; }

    public Guid SeccionGuid { get; set; }

    public string Proyecto { get; set; }

    public string Seccion { get; set; }

    public string Descripcion { get; set; }

    public string Fecha_Inicio_Proyectada { get; set; }

    public string Fecha_Terminacion_Proyectada { get; set; }

    public string Fecha_Inicio { get; set; }

    public string Fecha_Terminacion { get; set; }

    public string Contratista { get; set; }

    public bool? Terminado { get; set; }
}