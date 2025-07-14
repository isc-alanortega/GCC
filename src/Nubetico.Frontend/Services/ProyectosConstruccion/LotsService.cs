using System.Text;
using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion;

namespace Nubetico.Frontend.Services.ProyectosConstruccion
{
	public class LotsService
	{
		private readonly HttpClient _httpClient;

		public LotsService(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient("ApiClient");
		}

		public async Task<BaseResponseDto<object>?> GetPaginatedLots(int limit, int offset, FilterLots filter)
		{
			// Create the url request
			string endpoint = "api/v1/proyectosconstruccion/lotes/paginado";
			var queryParams = new Dictionary<string, string>
			{
				{ "limit", limit.ToString() },
				{ "offset", offset.ToString() }
			};

			var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
			var urlWithParams = $"{endpoint}?{queryString}";

			// Create the body
			var jsonContent = JsonConvert.SerializeObject(filter);
			var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

			// Send the request
			var response = await _httpClient.PostAsync(urlWithParams, content);

			// Read and deserialize the response
			var responseContent = await response.Content.ReadAsStringAsync();
			var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

			return dataResult;
		}

		public async Task<BaseResponseDto<object>?> GetLotByGuid(string uuid)
		{
			string endpoint = $"api/v1/proyectosconstruccion/lotes/{uuid}";

			var response = await _httpClient.GetAsync(endpoint);
			var responseContent = await response.Content.ReadAsStringAsync();
			var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

			return dataResult;
		}

		public async Task<BaseResponseDto<object>?> GetModelsList()
		{
			string endpoint = "api/v1/proyectosconstruccion/lotes/listado_modelos";

			var response = await _httpClient.GetAsync(endpoint);
			var responseContent = await response.Content.ReadAsStringAsync();
			var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

			return dataResult;
		}

		public async Task<BaseResponseDto<object>?> PostLot(LotsDetail lot)
		{
			string endpoint = "api/v1/proyectosconstruccion/lotes/postlote";

			var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
			var jsonContent = new StringContent(JsonConvert.SerializeObject(lot), Encoding.UTF8, "application/json");
			request.Content = jsonContent;

			var response = await _httpClient.SendAsync(request);
			var responseContent = await response.Content.ReadAsStringAsync();
			var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

			return dataResult;
		}

		public async Task<BaseResponseDto<object>?> UpdateLot(LotsDetail lot)
		{
			string endpoint = "api/v1/proyectosconstruccion/lotes/updatelote";

			var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
			var jsonContent = new StringContent(JsonConvert.SerializeObject(lot), Encoding.UTF8, "application/json");
			request.Content = jsonContent;

			var response = await _httpClient.SendAsync(request);
			var responseContent = await response.Content.ReadAsStringAsync();
			var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

			return dataResult;
		}

        public async Task<BaseResponseDto<bool?>?> CheckBlockInLotsByIdAsync(int blockId)
        {
            // Create the url request
            string endpoint = "api/v1/proyectosconstruccion/lotes/check-block-in-lot";
            var queryParams = new Dictionary<string, string>
            {
                { "blockId", blockId.ToString() }
            };

            var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
            var urlWithParams = $"{endpoint}?{queryString}";

            var response = await _httpClient.GetAsync(urlWithParams);
            var responseContent = await response.Content.ReadAsStringAsync();
            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<bool?>?>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<bool?>?> CheckStageInLotsByIdAsync(int stageId)
        {
            // Create the url request
            string endpoint = "api/v1/proyectosconstruccion/lotes/check-stage-in-lot";
            var queryParams = new Dictionary<string, string>
            {
                { "stageId", stageId.ToString() }
            };

            var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
            var urlWithParams = $"{endpoint}?{queryString}";

            var response = await _httpClient.GetAsync(urlWithParams);
            var responseContent = await response.Content.ReadAsStringAsync();
            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<bool?>?>(responseContent);

            return dataResult;
        }
    }
}
