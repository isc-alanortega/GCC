using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion
{
	public class DesatjosDto
	{
		public string Proyecto { get; set; } = string.Empty;
		public string Seccion { get; set; } = string.Empty;
		public string Contratista { get; set; } = string.Empty;
		public string FechaReporte { get; set; } = string.Empty;
		public string NotasReporte { get; set; } = string.Empty;
		public string Supervisor { get; set; } = string.Empty;
		public string FechaSupervision { get; set; } = string.Empty;
		public string Modelo { get; set; } = string.Empty;
		public string PorcentajeReportado { get; set; } = string.Empty;
		public string PorcentajeAprobado { get; set; } = string.Empty;
		public string NotasContratista { get; set; } = string.Empty;
		public string NotasSupervisor { get; set; } = string.Empty;
	}
}
