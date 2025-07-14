using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Nubetico.DAL.Models.Core;
using Nubetico.DAL.Providers.Core;
using Nubetico.DAL.ResultSets.Core;
using Nubetico.Shared.Dto.Core;
using Nubetico.WebAPI.Application.External.CIEmail;
using Nubetico.WebAPI.Application.Modules.Core.Models;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Application.Modules.Core.Services
{
    public class FormsService
    {
        private readonly CIEmailService _CIEmailService;
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;

        public FormsService(IDbContextFactory<CoreDbContext> coreDbContextFactory, CIEmailService ciEmailService)
        {
            _coreDbContextFactory = coreDbContextFactory;
            _CIEmailService = ciEmailService;
        }

        public async Task<FormRequestDto?> GetFormByAliasAsync(string alias)
        {
            using var coreDbContext = _coreDbContextFactory.CreateDbContext();
            Forms form = await coreDbContext.Forms.FirstOrDefaultAsync(x => x.Alias == alias) ?? throw new Exception("Formulario no encontrado.");
            Forms_Templates formTemplate = await coreDbContext.Forms_Templates.FirstOrDefaultAsync(m => m.IdFormTemplate == form.IdFormTemplate) ?? throw new Exception("Plantilla de formulario no encontrada.");
            List<FormRequestQuestionDto> questionsList = JsonConvert.DeserializeObject<List<FormRequestQuestionDto>>(formTemplate.Content) ?? throw new Exception("Error al deserealizar preguntas de formulario");

            FormRequestDto result = new FormRequestDto
            {
                Id = SqidsUtil.Encode(form.IdForm),
                Title = form.Titulo,
                WelcomeMessage = form.MensajeInicio,
                ImageUrl = form.UrlImagen,
                Questions = questionsList
            };

            return result;
        }

        public async Task<Tuple<bool, string>?> PostFormAsync(FormPostDto formPostDto, string direccionIp)
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            /// Validar que el form posteado sea válido 
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            int? idForm = SqidsUtil.Decode(formPostDto.Id);

            if (!idForm.HasValue)
                return null;

            using var coreDbContext = _coreDbContextFactory.CreateDbContext();
            Forms form = await coreDbContext.Forms.FirstOrDefaultAsync(m => m.IdForm == idForm.Value) ?? throw new Exception("Formulario no encontrado.");

            // Comenzar transaccion
            await using var transaction = await coreDbContext.Database.BeginTransactionAsync();

            try
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Generar una copia del objeto posteado y le resto los archivos b64 para serializarlo y guardarlo en BD.
                // Con esto obtenemos un folio (ID) de la respuesta.
                //////////////////////////////////////////////////////////////////////////////////////////////////////////
                FormPostDto preInsert = JsonConvert.DeserializeObject<FormPostDto>(
                    JsonConvert.SerializeObject(formPostDto)) ?? throw new Exception("");

                preInsert.Answers.RemoveAll(m => m.Type == "file");

                FolioResultSet folioResultSet = await FoliosProvider.GetFolioAsync(_coreDbContextFactory, "core.forms", null)
                    ?? throw new Exception("No se encontró una configuración válida en folidador");

                string folioSolicitud = $"{folioResultSet.Serie}{folioResultSet.Folio.ToString($"D{folioResultSet.Digitos}")}";

                Forms_Respuestas formRespuesta = new Forms_Respuestas
                {
                    Folio = folioSolicitud,
                    IdForm = form.IdForm,
                    DireccionIP = direccionIp,
                    Respuesta = JsonConvert.SerializeObject(preInsert),
                    FechaAlta = DateTime.UtcNow
                };

                // Guardar respuesta
                await coreDbContext.Forms_Respuestas.AddAsync(formRespuesta);

                if (await coreDbContext.SaveChangesAsync() <= 0)
                    throw new Exception("Se produjo un error al guardar las respuestas.");

                string mensajeRespuesta = form.MensajeRespuesta.Replace("__FOLIO__", folioSolicitud);

                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// Buscar si existen archivos posteados en el formulario y de existir almacenarlos
                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                List<FormPostAnswerDto> respuestasArchivos = formPostDto.Answers.Where(m => m.Type == "file").ToList();

                if (respuestasArchivos.Count > 0)
                {
                    // Parámetros de guardado de archivos
                    Parametros parametroRutaTrabajo = await coreDbContext.Parametros.FirstOrDefaultAsync(m => m.Alias == "archivos.rutaTrabajo") ?? throw new Exception("Parametro de ruta de trabajo no encontrado.");
                    string rutaGuardado = Path.Combine("forms-files", form.IdForm.ToString(), formRespuesta.IdFormRespuesta.ToString());

                    // Obtener los tipos de archivo permitidos
                    List<Archivos_Tipos> tiposArchivos = await coreDbContext.Archivos_Tipos.ToListAsync();

                    // Lista para almacenar las rutas de archivos posteados en bd.
                    List<Archivos> listaArchivos = new List<Archivos>();

                    foreach (var respuestaArchivo in respuestasArchivos)
                    {
                        FileInput64Dto archivoPosteado = JsonConvert.DeserializeObject<FileInput64Dto>(respuestaArchivo.Text) ?? throw new Exception($"Archivo {respuestaArchivo.Id} no válido.");

                        // Validar extensión de archivo
                        Archivos_Tipos tipoArchivo = tiposArchivos.FirstOrDefault(m => m.Extension == Path.GetExtension(archivoPosteado.FileName)) ?? throw new Exception($"Extensión {Path.GetExtension(archivoPosteado.FileName)} no permitida.");

                        Archivos nuevoArchivo = new Archivos
                        {
                            NombreOriginal = archivoPosteado.FileName,
                            RutaRelativa = Path.Combine(rutaGuardado, $"{respuestaArchivo.Id}{tipoArchivo.Extension}"),
                            IdArchivoTipo = tipoArchivo.IdArchivoTipo,
                            FechaAlta = DateTime.UtcNow
                        };

                        listaArchivos.Add(nuevoArchivo);

                        // Guardar archivo en el sistema
                        var rutaCompleta = Path.Combine(parametroRutaTrabajo.Valor1, nuevoArchivo.RutaRelativa);
                        await SaveFileB64Async(archivoPosteado.Content, rutaCompleta);
                    }

                    // Guardar archivos en la base de datos
                    await coreDbContext.Archivos.AddRangeAsync(listaArchivos);
                    await coreDbContext.SaveChangesAsync(); // Guardar antes para obtener los Id generados

                    // Crear la relación entre archivos y respuestas
                    var relacionesArchivos = listaArchivos.Select(m => new Forms_Respuestas_Archivos
                    {
                        IdArchivo = m.IdArchivo,
                        IdFormRespuesta = formRespuesta.IdFormRespuesta
                    }).ToList();

                    await coreDbContext.Forms_Respuestas_Archivos.AddRangeAsync(relacionesArchivos);

                    // Guardar las relaciones en la base de datos
                    if (await coreDbContext.SaveChangesAsync() <= 0)
                    {
                        throw new Exception("Se produjo un error al guardar las respuestas.");
                    }
                }

                if (form.EnviarCorreo)
                {
                    await EnviarExcelCorreoAsync(form, formPostDto, folioSolicitud);
                }

                await transaction.CommitAsync();

                return new Tuple<bool, string>(true, mensajeRespuesta);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        private async Task SaveFileB64Async(string base64Content, string ruta)
        {
            byte[] data = Convert.FromBase64String(base64Content);
            var directorio = Path.GetDirectoryName(ruta);

            if (!Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }

            await File.WriteAllBytesAsync(ruta, data);
        }

        private async Task<bool> EnviarExcelCorreoAsync(Forms form, FormPostDto formPostDto, string folioSolicitud)
        {
            // ********************************************************************************************************************************************************
            // Procesamos la info para enviar correo
            // ********************************************************************************************************************************************************
            List<string> destinatarios = form.Correos.Split(';').ToList();
            form.CuerpoCorreo = form.CuerpoCorreo.Replace("__FOLIO__", folioSolicitud);
            string bodyTXT = form.CuerpoCorreo;
            string bodyHTML = bodyTXT;
            List<EmailFileModel> nuevosArchivos = new List<EmailFileModel>();

            string excelBase64 = "";

            List<FormPostAnswerDto> archivos = formPostDto.Answers.Where(m => m.Type == "file").ToList();

            if (archivos.Count > 0)
            {
                foreach (var respuestaArchivo in archivos)
                {
                    FileInput64Dto archivoPosteado = JsonConvert.DeserializeObject<FileInput64Dto>(respuestaArchivo.Text) ?? throw new Exception($"Archivo no válido.");
                    EmailFileModel archivoNuevo = new EmailFileModel
                    {
                        ContentBase64 = archivoPosteado.Content,
                        FileName = $"{respuestaArchivo.Id}{Path.GetExtension(archivoPosteado.FileName)}"
                    };

                    nuevosArchivos.Add(archivoNuevo);
                }
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Datos");

                // Formateo de celdas
                worksheet.Column(1).Width = 35;
                worksheet.Column(2).Width = 60;

                // Especificar el tamaño de fuente y negrita
                var cell = worksheet.Cell("A1");
                cell.Style.Font.FontSize = 14;  // Tamaño de fuente
                cell.Style.Font.Bold = true;

                var cellA3 = worksheet.Cell("A3");
                cellA3.Style.Fill.BackgroundColor = XLColor.LightGray;
                cellA3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //cellA3.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                var cellB3 = worksheet.Cell("B3");
                cellB3.Style.Fill.BackgroundColor = XLColor.LightGray;
                cellB3.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //cellB3.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Agregar encabezado
                worksheet.Cell(1, 1).Value = $"Datos de la Solicitud {folioSolicitud}"; // form.AsuntoCorreo;
                worksheet.Cell(3, 1).Value = "Campo";
                worksheet.Cell(3, 2).Value = "Valor";

                // Agregamos respuestas
                int fila = 4;
                foreach (FormPostAnswerDto resp in formPostDto.Answers.Where(m => m.Type != "file").ToList())
                {
                    worksheet.Cell(fila, 1).Value = resp.Id;

                    if (resp.Type == "date")
                    {
                        if (!string.IsNullOrWhiteSpace(resp.Text))
                        {
                            worksheet.Cell(fila, 2).Value = DateTime.Parse(resp.Text);
                            worksheet.Cell(fila, 2).Style.NumberFormat.Format = "dd/MM/yyyy";
                            worksheet.Cell(fila, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        }
                    }
                    else
                    {
                        worksheet.Cell(fila, 2).Value = resp.Text;
                    }

                    fila++;
                }

                // Generar el memorystream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var bytes = stream.ToArray();
                    excelBase64 = Convert.ToBase64String(bytes);

                    nuevosArchivos.Add(new EmailFileModel { FileName = "Datos.xlsx", ContentBase64 = excelBase64 });
                }
            }

            form.AsuntoCorreo = form.AsuntoCorreo.Replace("__FOLIO__", folioSolicitud);
#if DEBUG
            form.AsuntoCorreo = $"[PRUEBAS] {form.AsuntoCorreo}";
#endif
            var resultEmail = await _CIEmailService.SendEmailAsync(destinatarios, form.AsuntoCorreo, bodyTXT, bodyHTML, nuevosArchivos);

            return resultEmail;
        }

    }
}
