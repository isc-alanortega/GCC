namespace Nubetico.Shared.Dto.ProyectosConstruccion
{
    public class InsumosDto
    {
        public int ID { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int? Id_Unit { get; set; }
        public string? Unit { get; set; }
        public int? Id_Type { get; set; }
        public string? Type { get; set; }
        public bool Enabled { get; set; }
    }

    public class InsumosModelos
    {
        public string Group { get; set; }
        public int GroupSecuence { get; set; }
        public string Category { get; set; }
        public int CategorySecuence { get; set; }
        public int Unit_Price_Id { get; set; }
        public string Unit_Price { get; set; }
        public int UnitPriceSecuence { get; set; }
        public string Supply { get; set; }
        public string Unit { get; set; }
        public string Type { get; set; }
        public decimal Volume { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }
}
