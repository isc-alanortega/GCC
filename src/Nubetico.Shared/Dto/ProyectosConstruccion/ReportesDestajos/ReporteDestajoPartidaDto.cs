namespace Nubetico.Shared.Dto.ProyectosConstruccion.ReportesDestajos
{
    public class ReporteDestajoPartidaDto
    {
        public int? IdReporteDestajoPartida { get; set; }
        public int IdModeloPU { get; set; }
        public string ModeloPU { get; set; }
        public int PorcentajeAvance { get; set; }
        public string Lotes { get; set; }
        public string Notas { get; set; }
        public List<ImagenDestajoDto> Imagenes { get; set; } = [];
    }
}
