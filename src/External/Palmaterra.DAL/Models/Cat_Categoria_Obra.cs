﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Palmaterra.DAL.Models;

public partial class Cat_Categoria_Obra
{
    public int IDCategoria { get; set; }

    public string Nombre { get; set; }

    public bool Habilitado { get; set; }

    public int IDTipoObra { get; set; }

    public virtual ICollection<Rel_Destajos_TiposTrabajo_CategoriaObra> Rel_Destajos_TiposTrabajo_CategoriaObra { get; set; } = new List<Rel_Destajos_TiposTrabajo_CategoriaObra>();

    public virtual ICollection<Rel_gastos_generales_trabajo> Rel_gastos_generales_trabajo { get; set; } = new List<Rel_gastos_generales_trabajo>();
}