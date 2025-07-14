namespace Nubetico.Shared.Dto.ProyectosConstruccion
{
    public class SubdivisionsDto
    {
        public Guid UUID { get; set; }
        public int? ID { get; set; }
		public string? Folio { get; set; }
        public string? Subdivision { get; set; }
        public string? PathLogo { get; set; }
        public int? PostalCode { get; set; }
        public bool? Enabled { get; set; }
        public IEnumerable<SubdivisionStage>? Stages { get; set; }
    }

    public class SubdivisionStage
    {
        public int ID { get; set; }
        public int? ID_Subdivision { get; set; }
        public string Stage { get; set; }
        public string? Description { get; set; }
        public int Sequence { get; set; }
		public IEnumerable<SubdivisionBlock>? Blocks { get; set; }
        public IEnumerable<LotsDto>? Lots { get; set; }
	}

    public class  SubdivisionBlock
    {
		public int ID { get; set; }
		public int? ID_Subdivision { get; set; }
        public int? ID_Subdivision_Stage { get; set; }
		public string Block { get; set; }
	}

    public class GeneralDetailSubdivision
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
