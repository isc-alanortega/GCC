using Nubetico.Shared.Dto.Core;

namespace Nubetico.Shared.Dto.ProyectosConstruccion
{
	public class LotsDto
	{
		public Guid? UUID { get; set; }
		public int ID { get; set; }
		public string? Folio { get; set; }
		public int? Number { get; set; }
		public DateTime? RegistrationDate { get; set; }
		public int? SubdivisionID { get; set; }
		public string? Subdivision { get; set; }
		public int? StageID { get; set; }
		public string? Stage { get; set; }
		public int? BlockID { get; set; }
		public string? Block { get; set; }
        public string? Model { get; set; }
        public string FrontMeasure { get; set; }
        public string BottomMeasure { get; set; }
        public string SurfaceMeasure { get; set; }
        public string? Status { get; set; }
    }

	public class LotsDetail
	{
		public Guid? UUID { get; set; }
		public int ID { get; set; }
		public string? Folio { get; set; }
		public int? Number { get; set; }
		public int? SubdivisionID { get; set; }
		public int? StageID { get; set; }
		public int? BlockID { get; set; }
		public double? FrontMeasure { get; set; }
		public double? BottomMeasure { get; set; }
		public double? SurfaceMeasure { get; set; }
		public int? ModelID { get; set; }
		public int? AddressID { get; set; }
		public DomicilioDto? Address { get; set; }
		public bool? Enabled { get; set; }
	}

	public class FilterLots
	{
		public int? SubdivisionID { get; set; }
		public int? StageID { get; set; }
		public int? BlockID { get; set; }
		public int? ModelID { get; set; }
		public int? LotNumber { get; set; }
		public int? StatusID { get; set; }
	}
}
