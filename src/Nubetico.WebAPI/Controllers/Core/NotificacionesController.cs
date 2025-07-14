using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Nubetico.Shared.Dto.Core;
using Nubetico.WebAPI.Application.Utils;
using Nubetico.WebAPI.Hubs;

namespace Nubetico.WebAPI.Controllers.Core
{
    [ApiController]
    [Authorize]
    [Route("api/v1/core/notificaciones")]
    public class NotificacionesController : ControllerBase
    {
        [HttpPost("enviar")]
        public async Task<IActionResult> SendNotification(
            [FromBody] NotificacionDto? notificacionDto,
            [FromServices] IHubContext<NotificacionesHub> hubContext)
        {
            if (notificacionDto == null)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest));

            await hubContext.Clients.All.SendAsync("ReceiveNotification", notificacionDto);

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, null, "Sent successfully"));
        }
    }
}
