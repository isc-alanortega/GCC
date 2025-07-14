using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using System.Text;

namespace Nubetico.Frontend.Services.Core
{
    public class FormsService
    {
        private readonly HttpClient _httpClient;

        public FormsService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<BaseResponseDto<object>> GetFormByAliasAsync(string alias)
        {
            string endpoint = $"api/v1/core/forms/id/{alias}";

            var response = await _httpClient.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<object>?> PostFormAsync(FormPostDto formPostDto, string turnstileToken)
        {
            string endpoint = "api/v1/core/forms";

            var content = new StringContent(
                JsonConvert.SerializeObject(formPostDto),
                Encoding.UTF8,
                "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = content
            };

            request.Headers.Add("x-turnstile-token", turnstileToken);

            var response = await _httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();
            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }
    }
}
