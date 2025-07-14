using Microsoft.AspNetCore.Components;
using Nubetico.Frontend.Services.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion.ProjectSectionDetails;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.ProyectosConstruccion.SeccionesProyectosComponents
{
    public partial class ProjectSectionLot : IDisposable
    {
        #region INJECTIONS
        [Inject] private GlobalBreakpointService? BreakpointService { get; set; }
        #endregion

        #region PARAMETER
        [Parameter] public List<SectionLotsDto> LotsData { get; set; }
        #endregion

        #region GRID PROPERTYS
        private RadzenDataGrid<SectionLotsDto>? LotsGrid { get; set; }
        public IList<SectionLotsDto> LotSelected { get; set; } = [];
        #endregion

        #region LIFE CICLE BLAZOR
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

        #region UTILS
        private bool GetIsReadOnly() => false;
        #endregion
    }
}
