namespace Nubetico.Shared.Dto.ProyectosConstruccion.Models
{
    public class ModelDto
    {
        public int? ModelId { get; set; }
        public string? Folio { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid? ModelGuid { get; set; }
        public bool? Enabled { get; set; } = false;
        public Guid? UserGuid { get; set; }
        public IEnumerable<InsumosModelos> ModelSupplies { get; set; } = [];
    }
}
