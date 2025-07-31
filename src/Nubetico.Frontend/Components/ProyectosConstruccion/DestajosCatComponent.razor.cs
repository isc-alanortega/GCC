using Nubetico.Frontend.Components.Shared;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class DestajosCatComponent: NbBaseComponent
    {
		private RadzenDataGrid<DesatjosDto>? GridDestajos { get; set; }
		private List<DesatjosDto>? ListaDestajos { get; set; }
		public IList<DesatjosDto> DestajosSeleccionados { get; set; } = [];
		private int Count { get; set; }
		private bool IsLoading { get; set; } = false;
		private int RowsPerPage { get; set; } = 10;


		DesatjosDto d1 = new DesatjosDto()
		{
			Proyecto = "PROYECTO 01",
			Seccion = "SE01",
			Contratista = "ARQ. MENDOZA",
			FechaReporte = "03/03/2025",
			NotasReporte = "OK",
			Supervisor = "OK",
			FechaSupervision = "04/03/2025",
			Modelo = "VILLA PLUS 2023",
			PorcentajeReportado = "35%",
			PorcentajeAprobado = "33%",
			NotasContratista = "",
			NotasSupervisor =""
		};

		DesatjosDto d2 = new DesatjosDto()
		{
			Proyecto = "PROYECTO 02",
			Seccion = "SE01",
			Contratista = "ARQ. ULISES",
			FechaReporte = "12/03/2025",
			NotasReporte = "OK",
			Supervisor = "OK",
			FechaSupervision = "14/03/2025",
			Modelo = "VILLA PLUS 2023",
			PorcentajeReportado = "78%",
			PorcentajeAprobado = "65%",
			NotasContratista = "",
			NotasSupervisor = ""
		};


		protected override async Task OnInitializedAsync()
		{
			TriggerMenuUpdate();

			ListaDestajos = new List<DesatjosDto> { d1, d2 };

		}

		private async Task LoadDataAsync(LoadDataArgs args)
		{
			//string orderBy = string.Join(",", args.Sorts.Select(s => $"{s.Property} {(s.SortOrder == SortOrder.Descending ? "desc" : "asc")}"));
			//await RefreshGridAsync(orderBy, args.Top ?? 0, args.Skip ?? 0);
		}

		private async Task DataGridRowDoubleClick(DataGridRowMouseEventArgs<DesatjosDto> args)
		{
			//if (args.Data != null)
			//    AbrirDetalleProveedor(args.Data, TipoEstadoControl.Lectura);
		}
	}
}
