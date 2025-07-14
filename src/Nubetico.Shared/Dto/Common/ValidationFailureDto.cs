namespace Nubetico.Shared.Dto.Common
{
    public class ValidationFailureDto
    {
        public string? PropertyName { get; set; }
        public string? ErrorMessage { get; set; }
        public object? AttemptedValue { get; set; }
        public object? CustomState { get; set; }
        public int Severity { get; set; }
        public string? ErrorCode { get; set; }
        public Dictionary<string, object>? FormattedMessagePlaceholderValues { get; set; }
    }
}
