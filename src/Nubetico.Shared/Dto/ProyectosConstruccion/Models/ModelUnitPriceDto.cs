using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Models
{
    public class ModelUnitPriceDto
    {
        public int? PriceUnitId { get; set; }
        public int? ModelBatchId { get; set; }
        public string? Description { get; set; }
        public bool? Enabled { get; set; }
        public List<ModelUnitPriceSupplyDto> Supplies { get; set; }

    }
}
