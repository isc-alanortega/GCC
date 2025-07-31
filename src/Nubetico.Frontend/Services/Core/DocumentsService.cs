using Newtonsoft.Json;
using Microsoft.AspNetCore.Components.Forms;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.PortalClientes;
using System.Net.Http.Json;


namespace Nubetico.Frontend.Services.Core
{
	public class DocumentsService
	{
		private readonly HttpClient _httpClient;

		public DocumentsService(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient("ApiClient");
		}

		public async Task<BaseResponseDto<ExcelResult<T>?>?> PostLoadExcel<T>(IBrowserFile file, string fileType)
		{
			string endpoint = "api/v1/core/documentos/validar_excel";


            var fileContent = new StreamContent(file.OpenReadStream());  // Forma correcta de crear el flujo de archivos
            fileContent.Headers.Add("Content-Type", "application/octet-stream");

            // Create the multipart form content
            var formData = new MultipartFormDataContent
			{
				/// Agregar el archivo a los datos del formulario
				{ fileContent, "File", file.Name },

				// Agregar el tipo de archivo a los datos del formulario
				{ new StringContent(fileType), "FileType" }
            };

			var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
			{
				Content = formData
			};

			var response = await _httpClient.SendAsync(request);
			var result = JsonConvert.DeserializeObject<BaseResponseDto<ExcelResult<T>?>?>(await response.Content.ReadAsStringAsync());

			return result;
		}

		public async Task<byte[]> DownloadInvoicePDF(ExternalClientInvoices invoice)
		{
            string endpoint = "api/v1/core/documentos/Post_DescargarFacturaPDF";

			var response = await _httpClient.PostAsJsonAsync(endpoint, invoice);
			if (!response.IsSuccessStatusCode)
			{
                throw new Exception(await response.Content.ReadAsStringAsync());
            }

			return await response.Content.ReadAsByteArrayAsync();
		}

		public async Task<byte[]> DownloadInvoiceZip(ExternalClientInvoices invoice)
		{
			string endpoint = "api/v1/core/documentos/Post_DescargarFacturaZIP";

			var response = await _httpClient.PostAsJsonAsync(endpoint, invoice);
			if (!response.IsSuccessStatusCode)
			{
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Ocurrió un error al descargar el ZIP: {error}");
			}

			return await response.Content.ReadAsByteArrayAsync();
		}
	}
}
