using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Nubetico.Frontend.Components.Core.Shared;
using Nubetico.Frontend.Services.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion.ProjectSectionDetails;

namespace Nubetico.Frontend.Components.ProyectosConstruccion.SeccionesProyectosComponents
{
    public partial class ProjectSectionDetailsComponent : NbBaseComponent, IDisposable
    {
        #region INJECTIONS
        [Inject] private GlobalBreakpointService? BreakpointService { get; set; }
        [Inject] private IStringLocalizer<SharedResources> Localizer { get; set; }
        #endregion

        [Parameter] public SectionDetailsDto Section { get; set; }

        #region LIFE CILCE BLAZOR
        protected override void OnInitialized()
        {
            BreakpointService!.OnChange += StateHasChanged;
            base.OnInitialized();
        }

        public void Dispose()
        {
            BreakpointService!.OnChange -= StateHasChanged;
        }
        #endregion
    }
}
