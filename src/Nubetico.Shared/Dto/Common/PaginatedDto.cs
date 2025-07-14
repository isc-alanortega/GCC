namespace Nubetico.Shared.Dto.Common
{
    public class PaginatedListDto<T>
    {
        //public int Draw { get; set; }
        public int RecordsTotal { get; set; } = 0;
        public int RecordsFiltered { get; set; } = 0;
        public List<T> Data { get; set; }
        public string Error { get; set; } = string.Empty;
    }
}
