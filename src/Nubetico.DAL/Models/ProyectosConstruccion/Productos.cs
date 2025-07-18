﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Nubetico.DAL.Models.ProyectosConstruccion;

public partial class Productos
{
    public int Id_Producto { get; set; }

    public string Descripcion { get; set; }

    public int Id_Tipo_Producto { get; set; }

    public int Id_SubTipo_Producto { get; set; }

    public int Id_Unidad_Entrada { get; set; }

    public int Id_Unidad_Salida { get; set; }

    public double Factor_Conversion { get; set; }

    public double Peso { get; set; }

    public decimal Costo { get; set; }

    public decimal Costo_Minimo { get; set; }

    public decimal Costo_Maximo { get; set; }

    public decimal Costo_Retenido { get; set; }

    public double Impuesto { get; set; }

    public string Observaciones { get; set; }

    public string Cuenta_Contable { get; set; }

    public bool Inventariable { get; set; }

    public DateTime Fecha_Alta { get; set; }

    public int Id_Usuario_Alta { get; set; }

    public DateTime? Fecha_Modificacion { get; set; }

    public int? Id_Usuario_Modificacion { get; set; }

    public Guid UUID { get; set; }

    public string Folio { get; set; }
}