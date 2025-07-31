using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Contratistas;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;
using System.Text;

namespace Nubetico.Frontend.Services.ProyectosConstruccion
{
    public class ContratistaService(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");
        private const string API_URL_BASE = "api/v1/proyectosconstruccion/contratistas";

        #region POST
        public async Task<BaseResponseDto<ContratistaResult>> PostSaveContratista(ContratistasDto model)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/PostSaveContratista";

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    return new BaseResponseDto<ContratistaResult>
                    {
                        StatusCode = (int)response.StatusCode,
                        Success = false,
                        Message = $"Error en la solicitud HTTP: {response.ReasonPhrase}",
                        ResponseKey = Guid.NewGuid(),
                        Data = null
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<ContratistaResult>>(responseContent);

                if (dataResult == null)
                {
                    return new BaseResponseDto<ContratistaResult>
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
                return new BaseResponseDto<ContratistaResult>
                {
                    StatusCode = 500,
                    Success = false,
                    Message = $"Error al crear el contratista: {ex.Message}",
                    ResponseKey = Guid.NewGuid(),
                    Data = null
                };
            }
        }
        #endregion
        #region GET
        public async Task<BaseResponseDto<PaginatedListDto<ContratistaGridResultSet>>> GetContratistaPaginadoAsync(int limit, int offset, string? orderBy, string? nombre, string? rfc)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/GetContratistaPaginado";
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
                var baseResponse = JsonConvert.DeserializeObject<BaseResponseDto<PaginatedListDto<ContratistaGridResultSet>>>(responseContent);
                return baseResponse ?? new BaseResponseDto<PaginatedListDto<ContratistaGridResultSet>>
                {
                    Success = false,
                    Message = "No se pudo deserializar la respuesta."
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<PaginatedListDto<ContratistaGridResultSet>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ContratistasDto?> GetContratistaByIdAsync(int idContratista)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/GetContratistaById{idContratista}";

                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress!, endpoint));
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                var baseResponse = JsonConvert.DeserializeObject<BaseResponseDto<ContratistasDto>>(responseContent);
                return baseResponse?.Data;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
        #region PUT
        public async Task<BaseResponseDto<ContratistaResult>> PutSaveContratista(ContratistasDto model)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/PutSaveContratista";
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    return new BaseResponseDto<ContratistaResult>
                    {
                        StatusCode = (int)response.StatusCode,
                        Success = false,
                        Message = $"Error en la solicitud HTTP: {response.ReasonPhrase}",
                        ResponseKey = Guid.NewGuid()
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<ContratistaResult>>(responseContent);

                if (dataResult == null)
                {
                    return new BaseResponseDto<ContratistaResult>
                    {
                        StatusCode = 500,
                        Success = false,
                        Message = "No se pudo deserializar la respuesta.",
                        ResponseKey = Guid.NewGuid()
                    };
                }

                dataResult.StatusCode = (int)response.StatusCode;
                return dataResult;
            }
            catch (Exception ex)
            {
                return new BaseResponseDto<ContratistaResult>
                {
                    StatusCode = 500,
                    Success = false,
                    Message = $"Error al actualizar el contratista: {ex.Message}, InnerException: {ex.InnerException?.Message}",
                    ResponseKey = Guid.NewGuid()
                };
            }
        }

        #endregion
    }
}
