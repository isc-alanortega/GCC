using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;

namespace Nubetico.Frontend.Services.Core
{
    public class MenuService
    {
        private readonly HttpClient _httpClient;

        public MenuService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<BaseResponseDto<object>> GetUserMenusAsync()
        {
            string endpoint = "api/v1/core/menu/user";

            var response = await _httpClient.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<object>> GetAllMenusAsync()
        {
            string endpoint = "api/v1/core/menu/all";

            var response = await _httpClient.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<object?>> GetAllMenuWithPermissionsAsync()
        {
            string endpoint = "api/v1/core/menu/all-permissions";

            var response = await _httpClient.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object?>>(responseContent);

            return dataResult;
        }
    }
}
