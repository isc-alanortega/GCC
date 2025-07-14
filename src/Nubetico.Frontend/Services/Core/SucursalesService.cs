using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;

namespace Nubetico.Frontend.Services.Core
{
    public class SucursalesService
    {
        private readonly HttpClient _httpClient;

        public SucursalesService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<BaseResponseDto<object>> GetSucursalesAsync()
        {
            string endpoint = "api/v1/core/sucursales";

            var response = await _httpClient.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }
    }
}
