using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.PortalClientes
{
    public class ExternalInvoicesFilter
    {
        public string? EntityContactGuid { get; set; }
        public string? Folio {  get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
