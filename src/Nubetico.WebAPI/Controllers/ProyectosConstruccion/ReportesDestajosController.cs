using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.ReportesDestajos;
using Nubetico.WebAPI.Application.Modules.ProyectosConstruccion.Services;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Controllers.ProyectosConstruccion
{
    [Route("api/v1/proyectosconstruccion/reportesdestajos")]
    [ApiController]
    [Authorize]
    public class ReportesDestajosController : ControllerBase
    {
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<List<ReporteDestajoGridDto>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetGridReportesDestajosAsync([FromQuery] int idSeccion, int? idStatus, [FromServices] ReportesDestajosService service)
        {
            var result = await service.GetGridReportesDestajosDtoAsync(idSeccion, idStatus);

            if (result == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));
            }

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result));
        }

        [HttpGet("{idReporteDestajo}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<List<BasicItemSelectDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetReporteDestajoDtoAsync([FromRoute] int? idReporteDestajo, [FromServices] ReportesDestajosService service)
        {
            if (!idReporteDestajo.HasValue)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest));

            ReporteDestajoDto? result = await service.GetReporteDestajoDtoAsync(idReporteDestajo.Value);

            if (result == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result));
        }

        [HttpGet("estatus-select")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<List<BasicItemSelectDto>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetReportesDestajosEstatusAsync([FromServices] ReportesDestajosService service)
        {
            var result = await service.GetReportesDestajosEstatusAsync();

            if (result == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));
            }

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result));
        }

        [HttpGet("project-select")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<List<BasicItemSelectDto>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetProjectSelectListAsync([FromServices] ReportesDestajosService service)
        {
            var result = await service.GetProjectSelectListAsync();

            if (result == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));
            }

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result));
        }

        [HttpGet("section-select")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<List<BasicItemSelectDto>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetSectionSelectListAsync([FromQuery] int projectId, [FromQuery] int? contractorId, [FromServices] ReportesDestajosService service)
        {
            var result = await service.GetSectionSelectListAsync(projectId, contractorId);

            if (result == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));
            }

            return StatusCode(StatusCodes.Status200OK, ResponseService.Response(StatusCodes.Status200OK, result));
        }

        [HttpGet("photo/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<List<BasicItemSelectDto>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> GetPhotoAsync(
            [FromServices] ReportesDestajosService service,
            [FromRoute] string id)
        {
            var result = await service.GetPhotoByIdAsync(id);

            if (result == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));

            return File(result.Item1, result.Item2, result.Item3);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponseDto<ReporteDestajoDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<List<ValidationFailure>>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> PostReporteDestajoDto(
           [FromServices] ReportesDestajosService service,
           [FromServices] IValidator<ReporteDestajoDto> validator,
           [FromBody] ReporteDestajoDto reporteDestajoDto)
        {
            var validate = await validator.ValidateAsync(reporteDestajoDto);

            if (!validate.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response(StatusCodes.Status400BadRequest, validate.Errors));

            ReporteDestajoDto result = await service.SetInsertReporteDestajoAsync(reporteDestajoDto);

            return StatusCode(StatusCodes.Status201Created, ResponseService.Response(StatusCodes.Status201Created, result));
        }

        [HttpPost("photo-upload/{tokenGuid}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponseDto<bool>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponseDto<object>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponseDto<object>))]
        public async Task<IActionResult> PhotoUploadAsync([FromServices] ReportesDestajosService service, [FromRoute] string tokenGuid, IFormFile file)
        {
            if (!Guid.TryParse(tokenGuid, out var guid))
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, "Guid no válido"));

            if (file == null || file.Length == 0)
                return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, "File cannot be null"));

            // Generar array de bytes
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            var result = await service.SetPhotoPartidaDestajo(guid, fileBytes, file.FileName);

            if (result == null)
                return StatusCode(StatusCodes.Status404NotFound, ResponseService.Response<object>(StatusCodes.Status404NotFound));

            return StatusCode(StatusCodes.Status201Created, ResponseService.Response(StatusCodes.Status201Created, true, ""));
        }

    }
}
