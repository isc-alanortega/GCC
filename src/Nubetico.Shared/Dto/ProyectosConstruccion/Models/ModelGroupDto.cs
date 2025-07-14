using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Models
{
    public class ModelGroupDto
    {
        public int? ModelId { get; set; }
        public int? GroupId { get; set; }
        public string? Description { get; set; }
        public bool? Enabled { get; set; }
        public IDictionary<string, ModelBatchDto> Batches { get; set; }
    }
}
