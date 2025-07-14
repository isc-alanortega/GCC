using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using System.Text;

namespace Nubetico.Frontend.Services.Core
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<BaseResponseDto<object>> GetAutenticacion(AuthRequestDto authDto)
        {
            string endpoint = "api/v1/core/auth";
            var content = JsonConvert.SerializeObject(authDto);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, bodyContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<object>> GetTokenVerifiedAsync(VerifyTokenRequestDto verifyTokenRequestDto)
        {
            string endpoint = "api/v1/core/auth/verify-token";
            var content = JsonConvert.SerializeObject(verifyTokenRequestDto);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, bodyContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<object>> PostNewPswdAsync(UpdatePswdByTokenDto updatePswdByTokenDto)
        {
            string endpoint = "api/v1/core/auth/update-auth";
            var content = JsonConvert.SerializeObject(updatePswdByTokenDto);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, bodyContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }
    }
}
