namespace Nubetico.Shared.Dto.Core
{
    public class MenuPermisosDto
    {
        public int IdMenu { get; set; }
        public int? IdMenuPadre { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Nivel { get; set; }
        public IEnumerable<MenuPermisosDto>? Children { get; set; } = Enumerable.Empty<MenuPermisosDto>();
        public List<PermisoDto> Permisos { get; set; }

    }

    public class PermisoDto
    {
        public int IdPermiso { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
