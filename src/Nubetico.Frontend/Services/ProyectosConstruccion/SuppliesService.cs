using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Nubetico.Shared.Dto.ProyectosConstruccion.Supplies;
using System.Text;

namespace Nubetico.Frontend.Services.ProyectosConstruccion
{
    public class SuppliesService(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");
        private const string API_URL_BASE = "api/v1/proyectosconstruccion/insumos";

        #region API ACCES
        #region GET
        public async Task<BaseResponseDto<PaginatedListDto<InsumosDto>?>?> GetPaginatedSupplies(SuppliesPaginatedRequestDto request)
        {
            string endpoint = $"{API_URL_BASE}/paginado";

            int limit = request.Limit!;
            int offset = request.Offset!;
            string? code = request.Code;
            string? description = request.Description;
            int? typeId = request.TypeId;

            var queryParams = new Dictionary<string, string>
            {
                { "limit", limit.ToString() },
                { "offset", offset.ToString() },
            };

            if(request.Code != null)
                queryParams.Add("code", request.Code);

            if (request.Description != null)
                queryParams.Add("description", request.Description);

            if (request.TypeId != null)
                queryParams.Add("typeId", request.TypeId!.Value.ToString());

            var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
            var urlWithParams = $"{endpoint}?{queryString}";

            var response = await _httpClient.GetAsync(urlWithParams);
            var responseContent = await response.Content.ReadAsStringAsync();
            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<PaginatedListDto<InsumosDto>?>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<SuppliesFetcherDto?>?> GetFetcherForm()
        {
            var response = await _httpClient.GetAsync($"{API_URL_BASE}/form");
            var responseContent = await response.Content.ReadAsStringAsync();
            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<SuppliesFetcherDto?>?>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<SuppliesDto?>?> GetSuppliesById(int suppliesId)
        {
            string endpoint = $"{API_URL_BASE}/found";
            var queryParams = new Dictionary<string, string> { [nameof(suppliesId)] = suppliesId.ToString() };

            var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
            var urlWithParams = $"{endpoint}?{queryString}";

            var response = await _httpClient.GetAsync(urlWithParams);
            var responseContent = await response.Content.ReadAsStringAsync();
            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<SuppliesDto?>>(responseContent);

            return dataResult;
        }

        public async Task<List<TablaRelacionDto>> GetAllTipoInsumo()
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/GetAllTipoInsumo";

                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress!, endpoint));
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                var baseResponse = JsonConvert.DeserializeObject<BaseResponseDto<List<TablaRelacionDto>>>(responseContent);
                return baseResponse?.Data ?? [];
            }
            catch (Exception)
            {
                return [];
            }
        }
        public async Task<List<TablaRelacionDto>> GetAllInsumos()
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/GetAllInsumos";

                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress!, endpoint));
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                var baseResponse = JsonConvert.DeserializeObject<BaseResponseDto<List<TablaRelacionDto>>>(responseContent);
                return baseResponse?.Data ?? [];
            }
            catch (Exception)
            {
                return [];
            }
        }
        #endregion

        #region POST
        public async Task<BaseResponseDto<SuppliesDto?>> PostAddSupplyAsync(SuppliesDto supply)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/add";

                var json = JsonConvert.SerializeObject(supply);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<SuppliesDto?>?>(responseContent);
                return dataResult!;
            }
            catch (Exception ex)
            {
                return GetDefaultErrorParseJson<SuppliesDto?>(ex);
            }
        }
        #endregion

        #region PATCH
        public async Task<BaseResponseDto<SuppliesDto?>> PatchEditSupplyAsync(SuppliesDto supply)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/edit";

                var json = JsonConvert.SerializeObject(supply);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<SuppliesDto?>?>(responseContent);
                return dataResult!;
            }
            catch (Exception ex)
            {
                return GetDefaultErrorParseJson<SuppliesDto?>(ex);
            }
        }

        public async Task<BaseResponseDto<bool?>> PatchDeleteSupplytAsync(int supplyId)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/delte";
                var queryParams = new Dictionary<string, string> { [nameof(supplyId)] = supplyId.ToString() };

                var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
                var urlWithParams = $"{endpoint}?{queryString}";

                var response = await _httpClient.GetAsync(urlWithParams);
                var responseContent = await response.Content.ReadAsStringAsync();

                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<bool?>>(responseContent);
                return dataResult!;
            }
            catch (Exception ex)
            {
                return GetDefaultErrorParseJson<bool?>(ex);
            }
        }
        #endregion

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
