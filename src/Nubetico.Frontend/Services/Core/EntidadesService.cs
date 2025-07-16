using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Supplies;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;
using Nubetico.Shared.Dto.Core;

namespace Nubetico.Frontend.Services.Core
{
	public class EntidadesService(IHttpClientFactory httpClientFactory)
	{
		private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");
		private const string API_URL_BASE = "api/v1/core/entidades";

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

			if (request.Code != null)
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

        //public async Task<BaseResponseDto<SuppliesDto?>?> GetSuppliesById(int entiddadId)
        //{
        //	string endpoint = $"{API_URL_BASE}/found";
        //	var queryParams = new Dictionary<string, string> { [nameof(suppliesId)] = suppliesId.ToString() };

        //	var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
        //	var urlWithParams = $"{endpoint}?{queryString}";

        //	var response = await _httpClient.GetAsync(urlWithParams);
        //	var responseContent = await response.Content.ReadAsStringAsync();
        //	var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<SuppliesDto?>>(responseContent);

        //	return dataResult;
        //}
        #endregion
        #region DATOS_FISCALES
        public async Task<List<TablaRelacionDto>> GetAllTipoRegimenFiscal()
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/GetAllTipoRegimen";

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

        public async Task<List<TablaRelacionDto>> GetAllRegimenFiscal()
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/GetAllRegimenFiscal";

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
        public async Task<List<TablaRelacionDto>> GetAllFormaPago()
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/GetAllFormaPago";

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
        public async Task<List<TablaRelacionStringDto>> GetAllMetodoDePago()
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/GetAllMetodoDePago";

                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress!, endpoint));
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                var baseResponse = JsonConvert.DeserializeObject<BaseResponseDto<List<TablaRelacionStringDto>>>(responseContent);
                return baseResponse?.Data ?? [];
            }
            catch (Exception)
            {
                return [];
            }
        } 
        public async Task<List<TablaRelacionStringDto>> GetAllUsoCFDI()
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/GetAllUsoCFDI";

                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress!, endpoint));
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                var baseResponse = JsonConvert.DeserializeObject<BaseResponseDto<List<TablaRelacionStringDto>>>(responseContent);
                return baseResponse?.Data ?? [];
            }
            catch (Exception)
            {
                return [];
            }
        }

        #endregion
        #endregion
    }
}
