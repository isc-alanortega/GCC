using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProveedoresFacturas;
using Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services;
using Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Controllers.ProveedoresFacturas
{
    [ApiController]
    [Authorize]
    [Route("api/v1/proveedoresfacturas/proveedor")]
    public class ClientesController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<Entidad_Simplificado>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]

        public async Task<IActionResult> GetProveedorAsync(
            [FromServices] ClientesServices proveedorService,
            [FromQuery] string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, ""));

            var result = await proveedorService.GetProvedorxCorreo(correo);

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<Entidad_Simplificado>(StatusCodes.Status200OK, result, ""));
        }

        [HttpPost("upload-invoice")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]

        public async Task<IActionResult> PostUploadnvoice(
            [FromServices] UploadInvoiceService invoiceService,
            [FromBody] UploadInvoiceRequestDto request)
        {
            var result = await invoiceService.SendInvoice(request.Invoice, request.ProviderData);
            if (!result.Success)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, message: result.Message));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<object>(StatusCodes.Status200OK, data: result));
        }
    }
}
