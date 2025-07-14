namespace Nubetico.Shared.Dto.ProyectosConstruccion.ReportesDestajos
{
    public class ReporteDestajoDto
    {
        public int? IdReporteDestajo { get; set; }
        public DateOnly FechaReporte { get; set; }
        public int? IdProyecto { get; set; }
        public string Proyecto { get; set; } = string.Empty;

        public int IdSeccion { get; set; }
        public string Seccion { get; set; } = string.Empty;

        public int IdStatus { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Glyph { get; set; } = string.Empty;
        public string DarkColorHex { get; set; } = string.Empty;
        public string LightColorHex { get; set; } = string.Empty;

        public int IdUsuarioAlta { get; set; }

        public List<ReporteDestajoPartidaDto> Partidas { get; set; } = [];
    }
}
