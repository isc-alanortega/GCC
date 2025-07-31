using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using System.Text;

namespace Nubetico.Frontend.Services.ProyectosConstruccion
{
    public class EgresosService
    {
        private readonly HttpClient _httpClient;

        public EgresosService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<BaseResponseDto<object>?> GetEgresosAsync()
        {
            string endpoint = "api/v1/proyectosconstruccion/egresos/";

            var response = await _httpClient.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }

        public async Task<BaseResponseDto<object>?> GetvEgresos_Partidas_DetallesAsync()
        {
            string endpoint = "api/v1/proyectosconstruccion/egresos/vEgresos_Partidas_Detalles";

            var response = await _httpClient.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }
    }
}
