using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProveedoresFacturas;
using System.Text;

namespace Nubetico.Frontend.Services.ProveedoresFacturas
{
    public class FacturasService
    {
        private readonly HttpClient _httpClient;

        public FacturasService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<BaseResponseDto<Entidad_Simplificado>?> GetProveedorAsync(string correo)
        {
            string endpoint = $"api/v1/proveedoresfacturas/proveedor";

            var queryParams = new Dictionary<string, string>
            {
                { "correo", correo }
            };

            var queryString = string.Join("&", queryParams.Select(param => $"{param.Key}={param.Value}"));
            var urlWithParams = $"{endpoint}?{queryString}";


            var response = await _httpClient.GetAsync(urlWithParams);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<Entidad_Simplificado>>(responseContent);

            return dataResult;
        }


        public async Task<BaseResponseDto<ResponseDto<object>>?> PostSendInvoiceProvider(UploadInvoiceRequestDto invoiceRequest)
        {
            string endpoint = $"api/v1/proveedoresfacturas/proveedor/upload-invoice";

            // Serializa el objeto a JSON
            var json = JsonConvert.SerializeObject(invoiceRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Envía la solicitud POST
            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<ResponseDto<object>>?>(responseContent);
            return dataResult;
        }

        public async Task<BaseResponseDto<object>> GetFacturasAsync(ApiFacturaPeticion peticion)
        {
            var content = JsonConvert.SerializeObject(peticion);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            string endpoint = $"api/v1/proveedoresfacturas/getfacturas";

            var response = await _httpClient.PostAsync(endpoint, bodyContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<object>>(responseContent);

            return dataResult;
        }
    }
}
