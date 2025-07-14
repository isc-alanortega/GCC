namespace Nubetico.Shared.Dto.Core
{
    public class AuthResponseDto
    {
        public PerfilUsuarioDto? PerfilUsuario { get; set; }
        public JwtDataDto? JwtData { get; set; }
        public bool TwoFactorRequired { get; set; }
        public List<RolDto> Roles { get; set; } = [];
    }
}
