using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.WebAPI.Application.Modules.Core.Services;
using Nubetico.WebAPI.Application.Utils;
using Nubetico.WebAPI.Application.Modules.Core.Models;
using Nubetico.Shared.Dto.PortalClientes;
using System.IO.Compression;
using DocumentFormat.OpenXml;
using System.IO;

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

        [HttpPost("Post_DescargarFacturaPDF")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post_DescargarFacturaPDF([FromServices] DocumentosService documentosService, [FromBody] ExternalClientInvoices invoice)
        {
			try
			{
                var FILEPATH = await documentosService.GetInvoiceUrlBasePath(invoice.Serial, invoice.Numeric_Folio, true);

                if (FILEPATH is null)
                {
                    return BadRequest("Path was null");
                }

                if (System.IO.File.Exists(FILEPATH))
                {
                    var stream = new FileStream(FILEPATH, FileMode.Open);
                    stream.Position = 0;
                    var fileName = $"Factura_{invoice.Folio}.pdf";

                    return File(stream, "application/pdf", fileName);
                }
                else
                {
                    return BadRequest("File does not exist");
                }
            } catch (Exception ex)
			{
                return BadRequest(string.Join("\n", ex.Message));
            }
        }

        [HttpPost("Post_DescargarFacturaZIP")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post_DescargarFacturaZIP([FromServices] DocumentosService documentosService, [FromBody] ExternalClientInvoices invoice)
        {
            try
            {
				var FILEPATHPDF = await documentosService.GetInvoiceUrlBasePath(invoice.Serial, invoice.Numeric_Folio, true);
				var FILEPATHPXML = await documentosService.GetInvoiceUrlBasePath(invoice.Serial, invoice.Numeric_Folio, false);

                if (FILEPATHPDF is null || FILEPATHPXML is null)
                {
                    return BadRequest("Path was null");
                }

                if (!System.IO.File.Exists(FILEPATHPDF) || !System.IO.File.Exists(FILEPATHPXML))
                {
                    return BadRequest("File doesn't exist");
                }

				var stream = new MemoryStream();
				var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true);

				// Add PDF
				var pdfEntry = zipArchive.CreateEntry($"Factura_{invoice.Folio}.pdf");
				using (var entryStream = pdfEntry.Open())
				using (var fileStream = System.IO.File.OpenRead(FILEPATHPDF))
				{
					await fileStream.CopyToAsync(entryStream);
				}

                // Add XML
                var xmlEntry = zipArchive.CreateEntry($"Factura_{invoice.Folio}.xml");
                using (var entryStream = xmlEntry.Open())
                using (var fileStream = System.IO.File.OpenRead(FILEPATHPXML))
                {
                    await fileStream.CopyToAsync(entryStream);
                }

                zipArchive.Dispose();
                stream.Position = 0;

                return File(stream, "application/zip", $"Factura_{invoice.Folio}.zip");
            }
            catch (Exception ex)
            {
                return BadRequest(string.Join("\n", ex.Message));
            }
        }
    }
}
