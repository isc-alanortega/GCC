namespace Nubetico.Shared.Dto.Core
{
    public class UsuarioExisteDirectoryDto
    {
        public Guid? GuidUsuario { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string PrimerApellido { get; set; } = string.Empty;
        public string SegundoApellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EnTenant { get; set; }
        public bool Confirmado { get; set; }
    }
}
