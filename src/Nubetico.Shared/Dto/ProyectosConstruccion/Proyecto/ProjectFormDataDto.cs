using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto
{
    /// <summary>
    /// Contains the necessary data to complete a project form
    /// </summary>
    public class ProjectFormDataDto
    {
        public IEnumerable<ElementsDropdownForm> Types { get; set; }
        public IEnumerable<ElementsDropdownForm> Branch { get; set; }
        public IEnumerable<ElementsDropdownForm> Subdivision { get; set; }
        public IEnumerable<ElementsDropdownForm> Users { get; set; }
        public IEnumerable<ElementsDropdownForm> Status { get; set; }
    }

    public class ElementsDropdownForm()
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Icon { get; set; }
    }
}
