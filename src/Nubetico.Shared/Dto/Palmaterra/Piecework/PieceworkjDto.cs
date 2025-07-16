using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.Palmaterra.Piecework
{
    public class PieceworkjDto
    {
        public int ResidentId { get; set; }
        public int TotalAmount { get; set; }
        public IEnumerable<PieceworkjElemntsDto> PieceworkElements { get; set; }
    }
}
