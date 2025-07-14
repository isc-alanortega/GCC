namespace Nubetico.Shared.Dto.Core
{
    public class FormRequestDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; }
        public string WelcomeMessage { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public List<FormRequestQuestionDto> Questions { get; set; } = [];
    }
}
