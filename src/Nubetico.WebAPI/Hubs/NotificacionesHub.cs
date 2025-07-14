using Microsoft.AspNetCore.SignalR;
using Nubetico.Shared.Dto.Core;

namespace Nubetico.WebAPI.Hubs
{
    public class NotificacionesHub : Hub
    {
        public async Task EnviarNotificacion(NotificacionDto notificacionDto)
        {
            await Clients.All.SendAsync("ReceiveNotification", notificacionDto);
        }
    }
}
