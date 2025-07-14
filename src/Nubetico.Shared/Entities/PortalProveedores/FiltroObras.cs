namespace Nubetico.Shared.Entities.PortalProveedores
{
    public class FiltroObras
    {
        public string? Nombre { get; set; }
        public int? IdPersonaResidente { get; set; }
        public int? IdEstadoObra { get; set; }
        public bool? EsProyectoObra { get; set; }
        public bool? ObraTerminada { get; set; }
    }
}
