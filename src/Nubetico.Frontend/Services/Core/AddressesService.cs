using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using System.Text;

namespace Nubetico.Frontend.Services.Core
{
	public class AddressesService
	{
		private readonly HttpClient _httpClient;

		public AddressesService(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient("ApiClient");
		}

		public async Task<BaseResponseDto<object>?> GetDomicilioByID(int id_domicilio)
		{
			string endpoint = "api/v1/core/domicilios/domiciliobyid";
			var queryParams = new Dictionary<string, string>
			{
				{ "id_domicilio", id_domicilio.ToString()}
			};

			var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
			var urlWithParams = $"{endpoint}?{queryString}";

			var response = await _httpClient.GetAsync(urlWithParams);
			var responseContent = await response.Content.ReadAsStringAsync();
			var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

			return dataResult;
		}

		public async Task<BaseResponseDto<object>?> GetEstadosListAsync()
		{
			string endpoint = "api/v1/core/domicilios/listado_estados";

			var response = await _httpClient.GetAsync(endpoint);
			var responseContent = await response.Content.ReadAsStringAsync();
			var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

			return dataResult;
		}

		public async Task<BaseResponseDto<object>?> GetMunicipiosListAsync(string? c_Estado = null, string? c_Municipio = null)
		{
			string endpoint = "api/v1/core/domicilios/listado_municipios";
			var queryParams = new Dictionary<string, string>
			{
				{ "c_Estado", c_Estado },
				{ "c_Municipio", c_Municipio }
			};

			var queryString = string.Join("&", queryParams.Select(param => $"{param.Key}={param.Value}"));
			var urlWithParams = $"{endpoint}?{queryString}";

			var response = await _httpClient.GetAsync(urlWithParams);
			var responseContent = await response.Content.ReadAsStringAsync();
			var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

			return dataResult;
		}

		public async Task<BaseResponseDto<object>?> GetColoniasListAsync(string? codigoPostal = null, string? filtro = null)
		{
			string endpoint = "api/v1/core/domicilios/listado_colonias";
			var queryParams = new Dictionary<string, string>
			{
				{ "codigoPostal", codigoPostal },
				{ "filtro", filtro }
			};

			var queryString = string.Join("&", queryParams.Select(param => $"{param.Key}={param.Value}"));
			var urlWithParams = $"{endpoint}?{queryString}";

			var response = await _httpClient.GetAsync(urlWithParams);
			var responseContent = await response.Content.ReadAsStringAsync();
			var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

			return dataResult;
		}

		public async Task<BaseResponseDto<object>?> GetCodigoPostalInfoAsync(string codigoPostal)
		{
			string endpoint = "api/v1/core/domicilios/codigopostal_informacion";
			var queryParams = new Dictionary<string, string>
			{
				{ "codigoPostal", codigoPostal }
			};

			var queryString = string.Join("&", queryParams.Select(param => $"{param.Key}={param.Value}"));
			var urlWithParams = $"{endpoint}?{queryString}";

			var response = await _httpClient.GetAsync(urlWithParams);
			var responseContent = await response.Content.ReadAsStringAsync();
			var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

			return dataResult;
		}

		public async Task<BaseResponseDto<object>?> PostDomicilio(DomicilioDto domicilio)
		{
			string endpoint = "api/v1/core/domicilios/postdomicilio";

			var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
			var jsonContent = new StringContent(JsonConvert.SerializeObject(domicilio), Encoding.UTF8, "application/json");
			request.Content = jsonContent;

			var response = await _httpClient.SendAsync(request);
			var responseContent = await response.Content.ReadAsStringAsync();
			var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

			return dataResult;
		}
	}
}
