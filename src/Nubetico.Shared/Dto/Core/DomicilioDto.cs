namespace Nubetico.Shared.Dto.Core
{
	public class DomicilioDto
	{
		public int ID { get; set; }
		public bool IsInvoincing { get; set; } = false;
		public string? Description { get; set; }
		public string? Street { get; set; }
		public string? StreetNumber { get; set; }
		public string? UnitNumber { get; set; }
		public string? ZipCode { get; set; }
		public string? c_State { get; set; }
		public string? c_City { get; set; }
		public string? c_Neighborhood { get; set; }
		public string? BetweenStreet1 { get; set; }
		public string? BetweenStreet2 { get; set; }
	}
}
