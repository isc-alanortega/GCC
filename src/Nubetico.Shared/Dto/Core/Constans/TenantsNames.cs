namespace Nubetico.WebAPI.Application.Modules.Core.Constans
{
    /// <summary>
    /// This class contains the names of the tenants (entities or clients) in the system.
    /// Tenants represent different clients or configurations within the system.
    /// The values are defined as static readonly constants.
    /// </summary>
    public static class TenantsNames
    {
        /// <summary>
        /// The name of the tenant for the Nubetico development environment.
        /// </summary>
        public const string CLIENTE_DEV = "nubetico dev";

        /// <summary>
        /// The name of the tenant for Moregar.
        /// </summary>
        public const string MOREGAR = "MOREGAR";

        /// <summary>
        /// The name of the tenant for GCC Contructora.
        /// </summary>
        public const string GCC = "GCC";
        public const string LOGEX = "LOGEX";
        public const string BLUEORANGE = "Blue Orange Pottery";
    }
}
