using Nubetico.Frontend.Components.Shared;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.Core.Shared
{
    public partial class ContactosComponent : NbBaseComponent
    {
        private RadzenDataGrid<ContactosDto>? GridContactos { get; set; }
        private List<ContactosDto>? ListaContactos { get; set; }
        public IList<ContactosDto> ContactosSeleccionados { get; set; } = [];
        private int Count { get; set; }
        private bool IsLoading { get; set; } = false;
        private int RowsPerPage { get; set; } = 10;


        ContactosDto c1 = new ContactosDto()
        {
            Nombre = "ARQ ULISES HERNANDEZ",
            Telefono = "312-123-4567",
            Correo = "uhernandez@ferrepacifico.com"
        };

        ContactosDto c2 = new ContactosDto()
        {
            Nombre = "ING. RAUL PEREZ",
            Telefono = "313-763-9381",
            Correo = "rperez@ferrepacifico.com"
        };

        ContactosDto c3 = new ContactosDto()
        {
            Nombre = "LORENA VELAZCO",
            Telefono = "312-323-3678",
            Correo = "lorena.velazco@ferreteriaelcaminante.com.mx"
        };

        ContactosDto c4 = new ContactosDto()
        {
            Nombre = "RIGOBERTO MANCHU",
            Telefono = "313-321-8590",
            Correo = "rmanchu@ferreterialugo.mx"
        };

        protected override async Task OnInitializedAsync()
        {
            TriggerMenuUpdate();

            ListaContactos = new List<ContactosDto> { c1, c2, c3, c4 };

        }

        private async Task LoadDataAsync(LoadDataArgs args)
        {
            //string orderBy = string.Join(",", args.Sorts.Select(s => $"{s.Property} {(s.SortOrder == SortOrder.Descending ? "desc" : "asc")}"));
            //await RefreshGridAsync(orderBy, args.Top ?? 0, args.Skip ?? 0);
        }

        private async Task DataGridRowDoubleClick(DataGridRowMouseEventArgs<ContactosDto> args)
        {
            //if (args.Data != null)
            //    AbrirDetalleProveedor(args.Data, TipoEstadoControl.Lectura);
        }
    }
}
