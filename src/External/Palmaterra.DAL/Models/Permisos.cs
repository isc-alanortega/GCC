﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Palmaterra.DAL.Models;

public partial class Permisos
{
    public int IDPermiso { get; set; }

    public int IDPadre { get; set; }

    public int? Nivel { get; set; }

    public string Descripcion { get; set; }

    public bool Editable { get; set; }
}