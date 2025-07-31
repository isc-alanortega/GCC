using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Controllers.ProyectosConstruccion
{
    [Route("api/v1/proyectosconstruccion/egresos/")]
    [ApiController]
    [Authorize]
    public class EgresosController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetEgresosAsync([FromServices] EgresosService egresosService)
        { 
            var result = await egresosService.GetEgresosAsync();
            if (result == null)
                return StatusCode(StatusCodes.Status204NoContent, ResponseService.Response<List<EgresoDto>?>(StatusCodes.Status204NoContent, result));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<List<EgresoDto>>(StatusCodes.Status200OK, result));
        }

        [HttpGet("vEgresos_Partidas_Detalles")]
        public async Task<IActionResult> GetvEgresos_Partidas_DetallesAsync([FromServices] EgresosService egresosService)
        {
            var result = await egresosService.GetvEgresos_Partidas_DetallesAsync();
            if (result == null)
                return StatusCode(StatusCodes.Status204NoContent, ResponseService.Response<List<vEgresos_Partidas_Detalles>?>(StatusCodes.Status204NoContent, result));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response<List<vEgresos_Partidas_Detalles>>(StatusCodes.Status200OK, result));
        }
    }
}
