namespace Nubetico.Shared.Dto.Core
{
    public class FormRequestQuestionDto
    {
        public string Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool Required { get; set; } = false;
        public int? MaxLength { get; set; }
        public List<string> Options { get; set; } = [];
        public string Answer { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
    }
}
