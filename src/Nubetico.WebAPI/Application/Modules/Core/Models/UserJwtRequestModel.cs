using Nubetico.Shared.Dto.Core;

namespace Nubetico.WebAPI.Application.Modules.Core.Models
{
    public class UserJwtRequestModel
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TenantGuid { get; set; } = string.Empty;
        public EntidadContactoUsuarioDto? EntidadContacto { get; set; }
    }
}
