namespace Nubetico.Shared.Dto.Core
{
    public class UsuarioNubeticoGridDto
    {
        public Guid UUID { get; set; }
        public string Username { get; set; }
        public string Nombres { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string? NombreCompleto { get; set; }
        public string Email { get; set; }
        public int IdEstadoUsuario { get; set; }
        public string EstadoUsuario { get; set; }
        public bool Bloqueado { get; set; }
    }
}
