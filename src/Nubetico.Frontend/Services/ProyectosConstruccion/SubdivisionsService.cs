using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using System.Text;

namespace Nubetico.Frontend.Services.ProyectosConstruccion
{
	public class SubdivisionsService
	{
		private readonly HttpClient _httpClient;

		public SubdivisionsService(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient("ApiClient");
		}

		public async Task<BaseResponseDto<object>?> GetPaginatedSubdivisions(int limit, int offset, string? filterName)
		{
			string endpoint = "api/v1/proyectosconstruccion/fraccionamientos/paginado";
			var queryParams = new Dictionary<string, string>
			{
				{ "limit", limit.ToString() },
				{ "offset", offset.ToString() },
				{ "filtername", filterName ?? ""}
			};

			var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
			var urlWithParams = $"{endpoint}?{queryString}";
			
			var response = await _httpClient.GetAsync(urlWithParams);
			var responseContent = await response.Content.ReadAsStringAsync();
			var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

			return dataResult;
		}

        public async Task<BaseResponseDto<object>?> GetSubdivisionByGuid(string uuid)
        {
            string endpoint = $"api/v1/proyectosconstruccion/fraccionamientos/{uuid}";

            var response = await _httpClient.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();
            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<object>?> GetSubdivisionsList()
		{
			string endpoint = "api/v1/proyectosconstruccion/fraccionamientos/listado";

			var response = await _httpClient.GetAsync(endpoint);
			var responseContent = await response.Content.ReadAsStringAsync();
			var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

			return dataResult;
		}

        public async Task<BaseResponseDto<object>?> GetStagesList(int? subdivisionID)
        {
            string endpoint = "api/v1/proyectosconstruccion/fraccionamientos/listado_etapas";
            var queryParams = new Dictionary<string, string>
            {
                { "subdivisionid", subdivisionID.HasValue ? subdivisionID.ToString() : null }
            };

            var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
            var urlWithParams = $"{endpoint}?{queryString}";

            var response = await _httpClient.GetAsync(urlWithParams);
            var responseContent = await response.Content.ReadAsStringAsync();
            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<object>?> GetBlocksList(int? subdivisionID, int? stageID)
        {
            string endpoint = "api/v1/proyectosconstruccion/fraccionamientos/listado_manzanas";

            var queryParams = new Dictionary<string, string>
            {
                { "subdivisionid", subdivisionID.HasValue ? subdivisionID.ToString() : null },
                { "stageid", stageID.HasValue ? stageID.ToString() : null }
            };

            var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
            var urlWithParams = $"{endpoint}?{queryString}";

            var response = await _httpClient.GetAsync(urlWithParams);
            var responseContent = await response.Content.ReadAsStringAsync();
            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }

		public async Task<BaseResponseDto<object>?> PostSubdivision(SubdivisionsDto subdivision)
		{
			string endpoint = "api/v1/proyectosconstruccion/fraccionamientos/postfraccionamiento";

			var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
			var jsonContent = new StringContent(JsonConvert.SerializeObject(subdivision), Encoding.UTF8, "application/json");
			request.Content = jsonContent;

			var response = await _httpClient.SendAsync(request);
			var responseContent = await response.Content.ReadAsStringAsync();
			var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

			return dataResult;
		}

		public async Task<BaseResponseDto<object>?> UpdateSubdivision(SubdivisionsDto subdivision)
		{
			string endpoint = "api/v1/proyectosconstruccion/fraccionamientos/updatefraccionamiento";

			var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
			var jsonContent = new StringContent(JsonConvert.SerializeObject(subdivision), Encoding.UTF8, "application/json");
			request.Content = jsonContent;

			var response = await _httpClient.SendAsync(request);
			var responseContent = await response.Content.ReadAsStringAsync();
			var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

			return dataResult;
		}
	}
}
