namespace Nubetico.Shared.Dto.Core
{
    public class MenuUsuarioDto
    {
        public bool New { get; set; }
        public bool Updated { get; set; }
        public string Name { get; set; }
        public string? Icon { get; set; }
        public string UnicodeIcon { get; set; }
        public string IconClass { get; set; }
        public string Path { get; set; }
        public string ComponentNamespace { get; set; }
        public string ComponentTypeName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Expanded { get; set; }
        public bool CanRepeatTab { get; set; }
        public IEnumerable<MenuUsuarioDto> Children { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public bool Enabled { get; set; }
    }
}
