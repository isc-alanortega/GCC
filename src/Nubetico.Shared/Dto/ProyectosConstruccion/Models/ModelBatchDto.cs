using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Models
{
    public class ModelBatchDto  
    {
        public int? BatchId { get; set; }
        public string? Description { get; set; }
        public int? ModelGroupId { get; set; }
        public bool? Enabled { get; set; }
        public IDictionary<string, ModelUnitPriceDto> UnitPrices { get; set; }
    }
}
