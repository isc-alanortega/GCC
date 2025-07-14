namespace Nubetico.Shared.Dto.Core
{
    public class UpdatePswdByTokenDto
    {
        public Guid Token { get; set; }
        public string Pswd { get; set; } = string.Empty;
        public string PswdConfirm { get; set; } = string.Empty;
    }
}
