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
        public async Task<BaseResponseDto<ProveedorResult>> PostSaveProveedor(ProveedorSaveDto model)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/add";

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    return new BaseResponseDto<ProveedorResult>
                    {
                        StatusCode = (int)response.StatusCode,
                        Success = false,
                        Message = $"Error en la solicitud HTTP: {response.ReasonPhrase}",
                        ResponseKey = Guid.NewGuid(),
                        Data = null
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<ProveedorResult>>(responseContent);

                if (dataResult == null)
                {
                    return new BaseResponseDto<ProveedorResult>
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "No se pudo deserializar la respuesta.",
                        ResponseKey = Guid.NewGuid(),
                        Data = null
                    };
                }

                dataResult.StatusCode = (int)response.StatusCode; // Asegurar StatusCode correcto
                dataResult.ResponseKey = dataResult.ResponseKey;

                return dataResult;
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<ProveedorResult>
                {
                    StatusCode = 500,
                    Success = false,
                    Message = $"Error al crear el proveedor: {ex.Message}",
                    ResponseKey = Guid.NewGuid(),
                    Data = null
                };
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
                string endpoint = $"{API_URL_BASE}/GetProveedorById{idProveedor}"; 

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

        public async Task<BaseResponseDto<PaginatedListDto<ProveedorGridResultSet>>> GetProveedoresPaginadoAsync(int limit, int offset, string? orderBy, string? nombre, string? rfc)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/GetProveedoresPaginado";
                var queryParams = new Dictionary<string,
                    string> {
                {
                    "limit",
                    limit.ToString()
                },
                {
                    "offset",
                    offset.ToString()
                }
            };
                if (!string.IsNullOrEmpty(nombre)) queryParams.Add("nombre", Uri.EscapeDataString(nombre));
                if (!string.IsNullOrEmpty(rfc)) queryParams.Add("rfc", Uri.EscapeDataString(rfc));
                if (!string.IsNullOrEmpty(orderBy)) queryParams.Add("orderBy", Uri.EscapeDataString(orderBy));
                var queryString = string.Join("&", queryParams.Select(param => $"{param.Key}={param.Value}"));
                var urlWithParams = $"{endpoint}?{queryString}";
                var response = await _httpClient.GetAsync(urlWithParams);
                var responseContent = await response.Content.ReadAsStringAsync();
                var baseResponse = JsonConvert.DeserializeObject<BaseResponseDto<PaginatedListDto<ProveedorGridResultSet>>>(responseContent);
                return baseResponse ?? new BaseResponseDto<PaginatedListDto<ProveedorGridResultSet>>
                {
                    Success = false,
                    Message = "No se pudo deserializar la respuesta."
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<PaginatedListDto<ProveedorGridResultSet>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        #endregion
        #region PUT
        public async Task<BaseResponseDto<ProveedorGridResultSet>> PutSaveProveedor(ProveedorSaveDto model)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/PutSaveProveedor";

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    return new BaseResponseDto<ProveedorGridResultSet>
                    {
                        StatusCode = (int)response.StatusCode,
                        Success = false,
                        Message = $"Error en la solicitud HTTP: {response.ReasonPhrase}",
                        ResponseKey = Guid.NewGuid(),
                        Data = null
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<ProveedorGridResultSet>>(responseContent);

                if (dataResult == null)
                {
                    return new BaseResponseDto<ProveedorGridResultSet>
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "No se pudo deserializar la respuesta.",
                        ResponseKey = Guid.NewGuid(),
                        Data = null
                    };
                }

                dataResult.StatusCode = (int)response.StatusCode; // Asegurar StatusCode correcto
                dataResult.ResponseKey = dataResult.ResponseKey;

                return dataResult;
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<ProveedorGridResultSet>
                {
                    StatusCode = 500,
                    Success = false,
                    Message = $"Error al actualizar el proveedor: {ex.Message}, InnerException: {ex.InnerException?.Message}",
                    ResponseKey = Guid.NewGuid(),
                    Data = null
                };
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
