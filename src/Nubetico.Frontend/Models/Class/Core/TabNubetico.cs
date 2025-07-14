using Microsoft.AspNetCore.Components;
using Nubetico.Frontend.Components.Core.Shared;
using Radzen.Blazor;
using static Nubetico.Frontend.Components.Core.Shared.NbBaseComponent;

namespace Nubetico.Frontend.Models.Class.Core
{
    public class TabNubetico
    {
        public TipoEstadoControl EstadoControl { get; set; } = TipoEstadoControl.Lectura;
        public RenderFragment Componente { get; set; }
        public string? Icono { get; set; }
        public NbBaseComponent InstanciaComponente { get; set; }
        public List<RadzenMenuItem> OpcionesMenu { get; set; } = new List<RadzenMenuItem>();
        public string Text { get; set; }
        public Type? TipoControl { get; set; }
        public bool Repetir { get; set; }
    }
}
