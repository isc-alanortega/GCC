using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nubetico.Shared.Dto.Palmaterra.Piecework
{
    public class PieceworkjElemntsDto
    {
        public Guid PiceworkUUID { get; set; }
        public int? ObraId { get; set; }
        public int? WorkId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

        /// <summary>
        /// Only for supervisor
        /// </summary>
        public int? ExpenseId { get; set; }
    }
}
