using Nubetico.Shared.Enums.Core;

namespace Nubetico.Shared.Dto.Core
{
    public class NotificacionDto
    {
        public RdzNotificationServerity Severidad { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public int DuracionMs { get; set; }
    }
}
