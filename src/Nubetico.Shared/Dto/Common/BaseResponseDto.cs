namespace Nubetico.Shared.Dto.Common
{
    public class BaseResponseDto<T>
    {
        public int StatusCode { get; set; }
        public Guid ResponseKey { get; set; } = Guid.NewGuid();
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
    }
}
