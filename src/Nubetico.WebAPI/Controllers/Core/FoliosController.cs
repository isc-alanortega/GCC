using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Providers.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core.Folios;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Controllers.Core
{
    [Route("api/v1/core/folios")]
    [ApiController]
    public class FoliosController : ControllerBase
    {

        /// <summary>
        /// Método para obtener el folio
        /// </summary>
        /// <param name="request"></param>
        /// <param name="dbFactory"></param>
        /// <returns></returns>
        [HttpPost("PostGetFolio")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<FolioResultSet>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> PostGetFolio(
            [FromBody] FolioRequestDto request,
            [FromServices] IDbContextFactory<CoreDbContext> dbFactory)
        {
            var result = await FoliosProvider.GetFolioAsync(dbFactory, request.Alias, request.IdSucursal);
            if (result == null)
                return NotFound(ResponseService.Response<object>(404, null, "Configuración de folio no encontrada"));

            return Ok(ResponseService.Response(200, result, string.Empty));
        }
    }
}
