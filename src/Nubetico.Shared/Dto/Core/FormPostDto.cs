namespace Nubetico.Shared.Dto.Core
{
    public class FormPostDto
    {
        public string Id { get; set; } = string.Empty;
        public List<FormPostAnswerDto> Answers { get; set; } = [];
    }
}
