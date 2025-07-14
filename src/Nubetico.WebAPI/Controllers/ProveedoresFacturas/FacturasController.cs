using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProveedoresFacturas;
using Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Controllers.ProveedoresFacturas
{
    [ApiController]
    [Authorize]
    [Route("api/v1/proveedoresfacturas/getfacturas")]
    public class FacturasController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<PaginatedListDto<FacturaDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]

        public async Task<IActionResult> GetFacturasAsync(
            [FromServices] FacturasServices facturasService,
            [FromBody] ApiFacturaPeticion peticion)
        {
            if (peticion == null)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, ""));

            PaginatedListDto<FacturaDto> result = await facturasService.GetFacturas(peticion);

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, result, ""));
        }
    }
}
