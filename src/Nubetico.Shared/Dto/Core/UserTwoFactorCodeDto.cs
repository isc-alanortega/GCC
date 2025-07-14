namespace Nubetico.Shared.Dto.Core
{
    public class UserTwoFactorCodeDto
    {
        public Guid GuidUsuario { get; set; }
        public string Key { get; set; }
        public string Code { get; set; }
    }
}
