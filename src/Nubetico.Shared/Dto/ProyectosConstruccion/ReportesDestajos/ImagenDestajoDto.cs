namespace Nubetico.Shared.Dto.ProyectosConstruccion.ReportesDestajos
{
    public class ImagenDestajoDto
    {
        public string Id { get; set; } = string.Empty;
        public string TokenUpload { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime? FechaCaptura { get; set; }
        public DateTime? FechaSincronizacion { get; set; }
    }
}
