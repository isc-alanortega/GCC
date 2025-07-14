namespace Nubetico.Shared.Dto.PortalProveedores
{
    public class ObraDto
    {
        public int IdObra { get; set; }
        public string Nombre { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? IdPersonaResidente { get; set; }
        public string Residente { get; set; }
        public int? IdEstadoObra { get; set; }
        public string EstadoObra { get; set; }
        public int? IdTipoObra { get; set; }
        public string TipoObra { get; set; }
        public bool? EsProyectoObra { get; set; }
        public bool? Completada { get; set; }
        public string Observaciones { get; set; }

    }
}
