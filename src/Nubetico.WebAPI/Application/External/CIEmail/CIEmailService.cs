using Newtonsoft.Json;
using Nubetico.WebAPI.Application.External.CIEmail.Dto;
using Nubetico.WebAPI.Application.Modules.Core.Models;
using System.Text;

namespace Nubetico.WebAPI.Application.External.CIEmail
{
    public class CIEmailService
    {
        private readonly HttpClient _httpClient;

        public CIEmailService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ci.smsbridge");
        }

        /// <summary>
        /// Función asyncrona para el envío de correos electrónicos.
        /// </summary>
        /// <param name="destinatarios">Lista de destinatarios, se espera cada destinatario tenga el siguiente formato "Saul Becerra CI <saul.becerra@centralinformatica.com>"</param>
        /// <param name="asunto">Asunto del correo</param>
        /// <param name="bodyText">Cuerpo del correo en formato texto plano</param>
        /// <param name="bodyHtml">Cuerpo del correo en formato HTML</param>
        /// <param name="archivos">Lista EmailFileModel que contiene el archivo en base 64 y el nombre del archivo.</param>
        /// <returns></returns>
        public async Task<bool> SendEmailAsync(List<string> destinatarios, string asunto, string bodyText, string bodyHtml, List<EmailFileModel>? archivos = null)
        {
            CIEmailRequestDto ciEmailModel = new CIEmailRequestDto
            {
                CorreoOrigen = "notificaciones",
                DominioOrigen = -1,
                CorreoDestinatario = string.Join(";", destinatarios),
                Asunto = asunto,
                MensajeTXT = bodyText,
                MensajeHTML = bodyHtml,
                NombresAdjuntos = new List<string>(),
                ContenidosAdjuntos = new List<string>()
            };

            if (archivos != null && archivos.Any())
            {
                foreach (var archivo in archivos)
                {
                    ciEmailModel.NombresAdjuntos.Add(archivo.FileName);
                    ciEmailModel.ContenidosAdjuntos.Add(archivo.ContentBase64);
                }
            }

            string bodyRequest = JsonConvert.SerializeObject(ciEmailModel);

            var stringContent = new StringContent(
                    bodyRequest,
                    Encoding.UTF8,
                    "application/json");

            var httpResponseMessage = await _httpClient.PostAsync("/api/CI_EnviarCorreosAdjuntos", stringContent);

            if (!httpResponseMessage.IsSuccessStatusCode)
                return false;

            string contentString = await httpResponseMessage.Content.ReadAsStringAsync();
            var ciEmailResponseDto = JsonConvert.DeserializeObject<CIEmailResponseDto>(contentString);

            return ciEmailResponseDto?.Estatus == "00";
        }
    }
}
