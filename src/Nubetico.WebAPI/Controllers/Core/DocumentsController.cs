using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.WebAPI.Application.Modules.Core.Services;
using Nubetico.WebAPI.Application.Utils;
using Nubetico.WebAPI.Application.Modules.Core.Models;

namespace Nubetico.WebAPI.Controllers.Core
{
	[ApiController]
	[Authorize]
	[Route("api/v1/core/documentos")]
	public class DocumentsController : ControllerBase
	{
		[HttpPost("validar_excel")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseDto<ExcelResult<IEnumerable<InsumosModelos>>>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponseDto<ExcelResult<string>>))]
		public async Task<IActionResult> PostLoadExcel([FromServices] DocumentosService documentosService, [FromForm] DocumentDto docuement)
		{
			try
			{
				if (docuement == null || docuement.File.Length == 0)
				{ 
					return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<ExcelResult<string>>
						(
							StatusCodes.Status400BadRequest,
							new ExcelResult<string>
							{
								Exception = "El archivo no contiene datos [WebApi.Controller]"
							},
							string.Empty
						)
					);
				}

				var result = documentosService.ValidateExcel(docuement.File, docuement.FileType);
				if (result.Result is null)
				{
					return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<ExcelResult<string>>
						(
							StatusCodes.Status400BadRequest, 
							new ExcelResult<string>
							{
								Exception = result.Exception,
								Error_Code = result.Error,
								Aditional_Error = result.Additional_Error_Data
							}, 
							string.Empty
						)
					);
				}

				return docuement.FileType switch
				{
					"MODELEXCEL" => StatusCode(StatusCodes.Status200OK, ResponseService.Response<ExcelResult<IEnumerable<InsumosModelos>>>
						(
							StatusCodes.Status200OK,
							new ExcelResult<IEnumerable<InsumosModelos>>
							{
								Result = (IEnumerable<InsumosModelos>)result.Result
							}
						)),
                    _ => StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<ExcelResult<string>>
						(
							StatusCodes.Status400BadRequest,
							new ExcelResult<string>
							{
								Exception = "No se encontró el tipo de excel [WebApi.Controller]."
							},
							string.Empty
						))
                };
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status400BadRequest, ResponseService.Response<ExcelResult<string>>
					(
						StatusCodes.Status400BadRequest,
						new ExcelResult<string>
						{
							Exception = string.Concat(ex.Message, "[WebApi.Controller]")
						},
						string.Empty
					)
				);
			}
		}
	}
}
