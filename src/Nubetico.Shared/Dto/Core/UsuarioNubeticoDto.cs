namespace Nubetico.Shared.Dto.Core
{
    public class UsuarioNubeticoDto
    {
        public Guid? UUID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string PrimerApellido { get; set; } = string.Empty;
        public string? SegundoApellido { get; set; } = string.Empty;
        public string? NombreCompleto { get; set; }
        public string Email { get; set; } = string.Empty;
        public int IdEstadoUsuario { get; set; }
        public string EstadoUsuario { get; set; } = string.Empty;
        public bool Bloqueado { get; set; } = false;
        public string? TwoFactorKey { get; set; } 
        public string? TwoFactorUrl {  get; set; }
        public ParametrosUsuarioDto Parametros { get; set; } = new ParametrosUsuarioDto();
        public IEnumerable<MenuDto> ListaMenus { get; set; } = Enumerable.Empty<MenuDto>();
        public List<PermisoAsignadoDto> Permisos { get; set; } = new List<PermisoAsignadoDto>();
        public List<SucursalDto> Sucursales { get; set; } = new List<SucursalDto>();
        public EntidadContactoUsuarioDto? EntidadContacto { get; set; }
    }
}
