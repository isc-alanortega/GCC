using Nubetico.Shared.Dto.Core;

namespace Nubetico.Frontend.Models.Class.Core
{
	public class DomicilioResult
	{
		public bool Success { get; set; }
		public string? ErrorMessage { get; set; }
		public DomicilioDto? Result { get; set; }
	}
}
