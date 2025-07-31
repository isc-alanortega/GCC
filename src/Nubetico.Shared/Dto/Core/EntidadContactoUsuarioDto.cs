using Nubetico.Shared.Enums.Core;

namespace Nubetico.Shared.Dto.Core
{
    public class EntidadContactoUsuarioDto
    {
        public Guid Id { get; set; }
        public TypeContactUserEnum? Tipo { get; set; }
        public string Rfc { get; set; } = string.Empty;
        public string Nombre { get; set; }
    }
}
