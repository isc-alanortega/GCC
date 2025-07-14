using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Nubetico.Frontend.Services.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion.ProjectSectionDetails;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.ProyectosConstruccion.SeccionesProyectosComponents
{
    public partial class ProjectSectionSuppliesComponent : IDisposable 
    {
        #region INJECTIONS
        [Inject] private GlobalBreakpointService? BreakpointService { get; set; }
        [Inject] protected IStringLocalizer<SharedResources>? Localizer { get; set; }
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



























        private int PhasesCount { get; set; }
        private RadzenDataGrid<ProjectSectionSuppliesModel>? SectionsGrid { get; set; }
        public IList<ProjectSectionSuppliesModel> PhasesSelected { get; set; } = [];
        public List<ProjectSectionSuppliesModel> Phases { get; set; } = [new() { Tipo = "1", Material = "Cemento", Estimado = "10 sacos", Real = "8 Sacos", Diferencia = "2 Sacos"}];


        private int PedidoCount { get; set; }
        private RadzenDataGrid<ProjectSectionSuppliesPedidoModel>? PedidoGrid { get; set; }
        public IList<ProjectSectionSuppliesPedidoModel> PedidoSelected { get; set; } = [];
        public List<ProjectSectionSuppliesPedidoModel> Pedido { get; set; } = [];
    }

    public class ProjectSectionSuppliesModel
    {
        public string Tipo { get; set; }
        public string Material { get; set; }
        public string Estimado { get; set; }
        public string Real { get; set; }
        public string Diferencia { get; set; }
    }


    public class ProjectSectionSuppliesPedidoModel
    {
        public string Numero { get; set; }
        public string Fecha { get; set; }
        public string Fase { get; set; }
        public string Solicita { get; set; }
        public string Material { get; set; }
        public string CantidadSolicitada { get; set; }
        public string CantidadAutorizada { get; set; }
        public string CantidadSurtida { get; set; }
        public string FechaSurtida { get; set; }
    }
}
