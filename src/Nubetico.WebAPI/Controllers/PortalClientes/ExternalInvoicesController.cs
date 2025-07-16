using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.PortalClientes;
using Nubetico.WebAPI.Application.Modules.PortalClientes.Services;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Controllers.PortalClientes
{
    [ApiController]
    [Authorize]
    [Route("api/v1/portalclientes/facturas")]
    public class ExternalInvoicesController : Controller
    {
        [HttpGet("externos")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<PaginatedListDto<ExternalClientInvoices>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetExternalInvoices([FromServices] ClientInvoicesService clientInvoicesService, [FromQuery] int limit, [FromQuery] int offset, [FromQuery] ExternalInvoicesFilter filter)
        {
            if (limit < 0 || offset < 0)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, ""));

            filter.EntityContactGuid = HttpContext.User.Claims.FirstOrDefault(user => user.Type == "entity-contact-id")?.Value;
            var result = await clientInvoicesService.GetExternalClientInvoices(limit, offset, filter);

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, result, ""));
        }
    }
}
