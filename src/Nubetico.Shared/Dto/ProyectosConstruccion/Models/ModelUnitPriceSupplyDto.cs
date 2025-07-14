using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.ProyectosConstruccion.Models
{
    public class ModelUnitPriceSupplyDto
    {
        public int? UnitPriceSupplyId { get; set; }
        public int? UnitPriceModelId { get; set; }
        public int? SupplyId { get; set; }
        public string? Supply { get; set; }
        public decimal? Volume { get; set; }
        public int? UnitId { get; set; }
        public string? Unit { get; set; }
        public string? Type { get; set; }
        public int? TypeId { get; set; }
        public decimal? Price { get; set; }
        public decimal? Amount { get; set; }
        public bool? Enabled { get; set; }
    }
}
