using DocumentFormat.OpenXml.Office2016.Excel;
using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Models;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;
using System.Net.Http;
using System.Text;

namespace Nubetico.Frontend.Services.ProyectosConstruccion
{
    public class ProveedorServices(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");
        private const string API_URL_BASE = "api/v1/proyectosconstruccion/proveedores";

        #region POST
        public async Task<BaseResponseDto<ProveedorSaveDto?>> PostSaveProveedor(ProveedorSaveDto model)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/add";

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<ProveedorSaveDto?>?>(responseContent);
                return dataResult!;
            }
            catch (Exception ex)
            {
                return GetDefaultErrorParseJson<ProveedorSaveDto?>(ex);
            }
        }
        #endregion
        #region GET
        public async Task<List<ProveedorGridResultSet>> GetAllProveedoresAsync()
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/GetAllProveedores";

                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress!, endpoint));
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                var baseResponse = JsonConvert.DeserializeObject<BaseResponseDto<List<ProveedorGridResultSet>>>(responseContent);
                return baseResponse?.Data ?? [];
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<ProveedorGetDto?> GetProveedorByIdAsync(int idProveedor)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/{idProveedor}"; 

                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress!, endpoint));
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                var baseResponse = JsonConvert.DeserializeObject<BaseResponseDto<ProveedorGetDto>>(responseContent);
                return baseResponse?.Data; 
            }
            catch (Exception)
            {
                return null; 
            }
        }

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
