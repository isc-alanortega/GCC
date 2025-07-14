namespace Nubetico.WebAPI.Application.External.Directory
{
    public static class DirectoryEndpoints
    {
        public const string Usuarios = "api/v1/usuario";
        public const string UsuarioValidar = "api/v1/usuario/validar";
        public const string UsuarioSetPassword = "api/v1/usuario/update-password";
        public const string UsuariosPorTenantPaginado = "/api/v1/usuario/tenant";
        public const string TenantsPorInstancia = "/api/v1/tenant/instancia";
    }
}
