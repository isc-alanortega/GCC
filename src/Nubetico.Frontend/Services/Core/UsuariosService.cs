using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using System.Text;

namespace Nubetico.Frontend.Services.Core
{
    public class UsuariosService
    {
        private readonly HttpClient _httpClient;

        public UsuariosService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<BaseResponseDto<object>?> GetUsuariosPaginado(int limit, int offset, string? orderBy, string? username, string? nombreCompleto, int? idEstadoUsuario)
        {
            string endpoint = "api/v1/core/usuarios/paginado";

            var queryParams = new Dictionary<string, string>
            {
                { "limit", limit.ToString() },
                { "offset", offset.ToString() }
            };

            if (!string.IsNullOrEmpty(username))
                queryParams.Add("username", Uri.EscapeDataString(username));

            if (!string.IsNullOrEmpty(nombreCompleto))
                queryParams.Add("nombreCompleto", Uri.EscapeDataString(nombreCompleto));

            if (idEstadoUsuario.HasValue)
                queryParams.Add("idEstadoUsuario", idEstadoUsuario.Value.ToString());

            if (!string.IsNullOrEmpty(orderBy))
                queryParams.Add("orderBy", Uri.EscapeDataString(orderBy));

            var queryString = string.Join("&", queryParams.Select(param => $"{param.Key}={param.Value}"));
            var urlWithParams = $"{endpoint}?{queryString}";

            var response = await _httpClient.GetAsync(urlWithParams);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<object>?> GetUsuarioByGuidAsync(string uuid)
        {
            string endpoint = $"api/v1/core/usuarios/{uuid}";

            var response = await _httpClient.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<object>?> PostUsuarioAsync(UsuarioNubeticoDto usuarioDto)
        {
            string endpoint = $"api/v1/core/usuarios";

            var content = new StringContent(
                    JsonConvert.SerializeObject(usuarioDto),
                    Encoding.UTF8,
                    "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<object>?> PutUsuarioAsync(UsuarioNubeticoDto usuarioDto)
        {
            string endpoint = $"api/v1/core/usuarios";

            var content = new StringContent(
                    JsonConvert.SerializeObject(usuarioDto),
                    Encoding.UTF8,
                    "application/json");

            var response = await _httpClient.PutAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<object>?> GetTokenUsuarioAsync(string uuid, bool newCode)
        {
            string endpoint = $"api/v1/core/usuarios/generar-qr?guidUsuario={uuid}&newCode={newCode.ToString().ToLower()}";

            var response = await _httpClient.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<object>?> PostValidarSetTokenUsuarioAsync(UserTwoFactorCodeDto userTwoFactorCodeDto)
        {
            string endpoint = $"api/v1/core/usuarios/set-token";

            var content = new StringContent(
                    JsonConvert.SerializeObject(userTwoFactorCodeDto),
                    Encoding.UTF8,
                    "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<object>?> GetUserStatusListAsync()
        {
            string endpoint = "api/v1/core/usuarios/select-estados";

            var response = await _httpClient.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<object>?> GetUsuarioExisteAsync(string username)
        {
            string endpoint = $"api/v1/core/usuarios/username/{username}";

            var response = await _httpClient.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }
    }
}
