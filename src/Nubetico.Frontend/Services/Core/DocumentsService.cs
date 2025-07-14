using Newtonsoft.Json;
using Microsoft.AspNetCore.Components.Forms;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.Common;


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
	}
}
