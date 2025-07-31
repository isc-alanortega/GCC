using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;

namespace Nubetico.Frontend.Services.Palmaterra
{
	public class ObrasServices
	{
		private readonly HttpClient _httpClient;

		public ObrasServices(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient("ApiClient");
		}

		public async Task<BaseResponseDto<object>?> GetObrasAsync()
		{
			string endpoint = "api/v1/palmaterra/obras";

			var response = await _httpClient.GetAsync(endpoint);
			var responseContent = await response.Content.ReadAsStringAsync();

			var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

			return dataResult;
		}
	}
}
