namespace Nubetico.WebAPI.Application.External.Directory.Dto
{
    public class UsuarioDto
    {
        public Guid? UUID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Nombres { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Email { get; set; }
        public bool? Habilitado { get; set; }
        public List<EnrolamientoTenantDto>? TenantsEnrolados { get; set; }
    }
}
