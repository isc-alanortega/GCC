namespace Nubetico.Shared.Dto.Core
{
    public class MenuDto
    {
        public int IdMenu { get; set; }
        public int? IdMenuPadre { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Nivel { get; set; }
        public IEnumerable<MenuDto>? Children { get; set; } = Enumerable.Empty<MenuDto>();
        public bool Habilitado { get; set; } = true;
        public bool Seleccionable { get; set; } = false;
        public bool Check { get; set; } = false;
    }
}
