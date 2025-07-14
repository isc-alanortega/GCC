using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;

namespace Nubetico.Frontend.Services.PortalProveedores
{
    public class ObraService
    {
        private readonly HttpClient _httpClient;

        public ObraService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<BaseResponseDto<object>> GetPaginadoAsync(int limit, int offset, string orderBy)
        {
            string endpoint = $"api/v1/portalproveedores/obras/paginado";

            var queryParams = new Dictionary<string, string>
            {
                { "limit", limit.ToString() },
                { "offset", offset.ToString() }
            };

            if (!string.IsNullOrEmpty(orderBy))
                queryParams.Add("orderBy", Uri.EscapeDataString(orderBy));

            var queryString = string.Join("&", queryParams.Select(param => $"{param.Key}={param.Value}"));
            var urlWithParams = $"{endpoint}?{queryString}";


            var response = await _httpClient.GetAsync(urlWithParams);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }
    }
}
