namespace Nubetico.WebAPI.Application.Modules.Core.Models
{
    public class TenantModel
    {
        public Guid TenantGuid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string TenantUrl { get; set; } = string.Empty;
    }
}
