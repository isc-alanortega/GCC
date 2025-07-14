namespace Nubetico.Shared.Dto.Core
{
	public class ExcelResult<T>
	{
		public T Result { get; set; }
		public string? Exception { get; set; }
		public string? Error_Code { get; set; }
		public string? Aditional_Error { get; set; }
	}
}