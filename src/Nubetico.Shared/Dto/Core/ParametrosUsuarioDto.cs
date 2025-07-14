namespace Nubetico.Shared.Dto.Core
{
    public class ParametrosUsuarioDto
    {
        public bool NavegarPorTabs { get; set; } = true;
        public int? IdMenuInicio { get; set; }
        public string? CulturaDefault { get; set; } = "es-MX";
        public string? ZonaHorariaDefault { get; set; } = "America/Mexico_City";
    }
}
