using Nubetico.Frontend.Components.Shared;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class ComprasCatComponent : NbBaseComponent
    {
        private RadzenDataGrid<ComprasDto>? GridCompras { get; set; }
        private List<ComprasDto>? ListaCompras { get; set; }
        public IList<ComprasDto> ComprasSeleccionadas { get; set; } = [];
        private int Count { get; set; }
        private bool IsLoading { get; set; } = false;
        private int RowsPerPage { get; set; } = 10;


        ComprasDto c1 = new ComprasDto()
        {
            Fecha = "03/03/2025",
            Proveedor = "FERREPACIFICO",
            Importe = "87,500"
        };

        ComprasDto c2 = new ComprasDto()
        {
            Fecha = "07/03/2025",
            Proveedor = "FERREPACIFICO",
            Importe = "34,560"
        };

        ComprasDto c3 = new ComprasDto()
        {
            Fecha = "06/03/2025",
            Proveedor = "FERRETERIA LUGO",
            Importe = "12,790"
        };

        ComprasDto c4 = new ComprasDto()
        {
            Fecha = "11/03/2025",
            Proveedor = "FERRETERIA EL CAMINANTE",
            Importe = "29,003"
        };

        protected override async Task OnInitializedAsync()
        {
            TriggerMenuUpdate();

            ListaCompras = new List<ComprasDto> { c1, c2, c3, c4 };

        }

        private async Task LoadDataAsync(LoadDataArgs args)
        {
            //string orderBy = string.Join(",", args.Sorts.Select(s => $"{s.Property} {(s.SortOrder == SortOrder.Descending ? "desc" : "asc")}"));
            //await RefreshGridAsync(orderBy, args.Top ?? 0, args.Skip ?? 0);
        }

        private async Task DataGridRowDoubleClick(DataGridRowMouseEventArgs<ComprasDto> args)
        {
            //if (args.Data != null)
            //    AbrirDetalleProveedor(args.Data, TipoEstadoControl.Lectura);
        }
    }
}
