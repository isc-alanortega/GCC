namespace Nubetico.Shared.Dto.Core
{
    public class PermisoAsignadoDto
    {
        public int IdPermiso { get; set; }
        public string Permiso { get; set; } = string.Empty;
        public int IdSucursal { get; set; }
        public string Sucursal { get; set; } = string.Empty;
        public int IdMenu { get; set; }
    }
}
