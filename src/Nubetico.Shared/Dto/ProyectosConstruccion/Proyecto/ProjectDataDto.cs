using Nubetico.Shared.Enums.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto
{
    public class ProjectDataDto
    {
        /// <summary>
        /// Gets or sets (IdProyecto) the unique identifier for the project.
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the identifier (IdSucursal) for the branch or office related to the project.
        /// </summary>
        public int? BranchId { get; set; }

        /// <summary>
        /// Gets or sets the identifier (IdFraccionamiento) for the development 
        /// </summary>
        public int? SubdivisionId { get; set; }

        /// <summary>
        /// Gets or sets the identifier (IdTipo) for the project type.
        /// </summary>
        public int? TypeId { get; set; }

        public int? TotalUnits { get; set; }

        public int? StatusId { get; set; }

        public string? Folio {  get; set; }

        public Guid? ProjectGuid { get; set; }

        /// <summary>
        /// Gets or sets (Nombre) the name of the project.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets (Descripcion) a brief description of the project.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets (IdEncargado) the identifier of the person responsible for the project.
        /// </summary>
        public int? ResponsibleId { get; set; }

        /// <summary>
        /// Gets or sets (FechaInicioProyectada) the projected start date of the project.
        /// </summary>
        public DateTime? ProjectedStartDate { get; set; }

        /// <summary>
        /// Gets or sets (FechaTerminacionProyectada) the projected end date of the project.
        /// </summary>
        public DateTime? ProjectedEndDate { get; set; }

        /// <summary>
        /// Gets or sets the actual start date of the project.
        /// </summary>
        public DateTime? ActualStartDate { get; set; }

        /// <summary>
        /// Gets or sets the actual end date of the project.
        /// </summary>
        public DateTime? ActualEndDate { get; set; }

        ///// <summary>
        ///// Gets or sets the identifier of the user who performed the action (either creation or modification).
        ///// </summary>
        //public ActionFormDto? ActionForm { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who performed the action (either creation or modification).
        /// </summary>
        public Guid? ActionUserGuid { get; set; }

        public List<ProjectSectionDataDto> ProjectSectionData { get; set; } = [];
    }
}
