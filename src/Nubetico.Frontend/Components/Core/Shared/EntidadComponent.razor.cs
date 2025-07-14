using Microsoft.AspNetCore.Components;
using Nubetico.Shared.Dto.ProyectosConstruccion;

namespace Nubetico.Frontend.Components.Core.Shared
{
	public partial class EntidadComponent
	{
		#region Propiedades

		[Parameter]  public ProveedoresDto? ProveedorData { get; set; }

		#endregion

		#region Funciones

		private bool GetDisabled(string? field_name = null)
		{
			return EstadoControl == TipoEstadoControl.Lectura;
			//return false;
		}

		#endregion
	}
}
