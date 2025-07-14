using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Models;
using System.Text;

namespace Nubetico.Frontend.Services.ProyectosConstruccion
{
    public class ModelService(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");
        private const string API_URL_BASE = "api/v1/proyectosconstruccion/models";

        #region API ENDPOINTS METHODS
        #region GET
        public async Task<BaseResponseDto<PaginatedListDto<ModelGridDto>?>?> GetPaginatedModelsAsync(ModelGridRequestDto request)
        {
            string endpoint = $"{API_URL_BASE}/paginado";

            var queryParams = new Dictionary<string, string>
            {
                { "limit", request.Limit.ToString() },
                { "offset", request.Offset.ToString() },
            };

            if (request.Name != null)
                queryParams.Add("name", request.Name);

            var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
            var urlWithParams = $"{endpoint}?{queryString}";

            var response = await _httpClient.GetAsync(urlWithParams);
            var responseContent = await response.Content.ReadAsStringAsync();
            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<PaginatedListDto<ModelGridDto>?>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<ModelDto?>> GetModelByIdAsync(int modelId)
        {
            string endpoint = $"{API_URL_BASE}/found";

            var queryParams = new Dictionary<string, string>
            {
                { nameof(modelId), modelId.ToString() }
            };

            var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
            var urlWithParams = $"{endpoint}?{queryString}";

            var response = await _httpClient.GetAsync(urlWithParams);
            var responseContent = await response.Content.ReadAsStringAsync();
            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<ModelDto?>>(responseContent);

            return dataResult;
        }
        #endregion

        #region POST
        public async Task<BaseResponseDto<ModelDto?>> PostAddModelAsync(ModelDto model)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/add";

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<ModelDto?>?>(responseContent);
                return dataResult!;
            }
            catch (Exception ex)
            {
                return GetDefaultErrorParseJson<ModelDto?>(ex);
            }
        }
        #endregion

        #region PATCH

        #endregion
        #endregion

        #region UTILS
        public BaseResponseDto<T?> GetDefaultErrorParseJson<T>(Exception ex) => new BaseResponseDto<T?>
        {
            StatusCode = 501,
            Success = false,
            Message = "Shared.Core.UnknowError",
            Data = default
        };
        #endregion
    }
}
