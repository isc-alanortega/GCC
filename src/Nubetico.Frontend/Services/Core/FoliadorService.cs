using System.Text;
using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.Core.Folios;

namespace Nubetico.Frontend.Services.Core
{
    public class FoliadorService(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");
        private const string API_URL_BASE = "api/v1/core/folios";


        /// <summary>
        /// Obtiene un nuevo folio desde el servidor.
        /// </summary>
        public async Task<FolioResultSet?> PostGetFolioAsync(FolioRequestDto request)
        {
            var endpoint = $"{API_URL_BASE}/PostGetFolio";
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var wrapper = JsonConvert.DeserializeObject<BaseResponseDto<FolioResultSet>>(responseContent);
            return wrapper?.Data;
        }
    }
}
