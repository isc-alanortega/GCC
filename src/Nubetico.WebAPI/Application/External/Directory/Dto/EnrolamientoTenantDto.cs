namespace Nubetico.WebAPI.Application.External.Directory.Dto
{
    public class EnrolamientoTenantDto
    {
        public Guid UUID { get; set; }
        public string? Nombre { get; set; }
        public bool? EnrolamientoConfirmado { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }
        public bool? BajaVoluntaria { get; set; }
        public bool? Baja { get; set; }
    }
}
