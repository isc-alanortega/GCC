using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nubetico.DAL.Models.ProyectosConstruccion;
using Nubetico.WebAPI.Application.Modules.Palmaterra.Service;
using Nubetico.WebAPI.Application.Utils;
using Palmaterra.DAL.Models;

namespace Nubetico.WebAPI.Controllers.Palmaterra
{
	[Route("api/v1/palmaterra/")]
	[ApiController]
	[Authorize]
	public class ObrasController : ControllerBase
	{
		[HttpGet("obras")]
		public async Task<IActionResult> GetObraAsync([FromServices] PalmaterraService palmaterraService)
		{
			var result = await palmaterraService.GetObrasAsync();
			if (result == null)
				return StatusCode(StatusCodes.Status204NoContent, ResponseService.Response<List<Obras>?>(StatusCodes.Status204NoContent, result));

			return StatusCode(StatusCodes.Status200OK, ResponseService.Response<List<Obras>>(StatusCodes.Status200OK, result));
		}
	}
}
