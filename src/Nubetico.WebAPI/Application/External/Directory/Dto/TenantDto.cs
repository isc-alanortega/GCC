namespace Nubetico.WebAPI.Application.External.Directory.Dto
{
    public class TenantDto
    {
        public Guid? UUID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string CadenaConexion { get; set; } = string.Empty;
        public List<string>? WebUrls { get; set; }
        public Guid InstanciaUUID { get; set; }
        public DateTime? FechaAlta { get; set; }
        public bool? Habilitado { get; set; }
    }
}
