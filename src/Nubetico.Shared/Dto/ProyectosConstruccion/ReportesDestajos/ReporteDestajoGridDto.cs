namespace Nubetico.Shared.Dto.ProyectosConstruccion.ReportesDestajos
{
    public class ReporteDestajoGridDto
    {
        public int IdReporteDestajo { get; set; }
        public int IdSeccion { get; set; }
        public string Seccion { get; set; } = string.Empty;
        public DateOnly Fecha { get; set; }
        public int IdStatus { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Glyph { get; set; } = string.Empty;
        public string LightColorHex { get; set; } = string.Empty;
        public string DarkColorHex { get; set; } = string.Empty;
    }
}
