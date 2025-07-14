using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto
{
    public class ProyectsGridDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the project (ProyectoGuid).
        /// </summary>
        public Guid? ProjectGuid { get; set; }

        /// <summary>
        /// Gets or sets the business unit (UnidadNegocio).
        /// </summary>
        public string BusinessUnit { get; set; }

        /// <summary>
        /// Gets or sets the housing development (Fraccionamiento).
        /// </summary>
        public string Subdivision { get; set; }

        /// <summary>
        /// Gets or sets the type of the project (Tipo).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the project (Nombre).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the projected start date of the project (FechaInicioProyectada).
        /// </summary>
        public string? ProjectedStartDate { get; set; }

        /// <summary>
        /// Gets or sets the projected end date of the project (FechaFinProyectada).
        /// </summary>
        public string? ProjectedEndDate { get; set; }

        /// <summary>
        /// Gets or sets the current status of the project (Estado).
        /// </summary>
        public string State { get; set; }

        public int StateId { get; set; }

        /// <summary>
        /// Gets or sets the reference number for the project (Folio).
        /// </summary>
        public string Folio { get; set; }
    }
}
