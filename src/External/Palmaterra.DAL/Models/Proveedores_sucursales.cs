﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Palmaterra.DAL.Models;

public partial class Proveedores_sucursales
{
    public int IDSucursal { get; set; }

    public int? IDProveedor { get; set; }

    public string Nombre { get; set; }

    public string Usuario { get; set; }

    public string Contraseña { get; set; }

    public bool? Activo { get; set; }

    public string Email { get; set; }

    public int? IDLugarSucursal { get; set; }
}