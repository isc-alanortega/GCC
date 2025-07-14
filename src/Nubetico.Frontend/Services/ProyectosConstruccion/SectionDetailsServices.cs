using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.ProjectSectionDetails;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using System.Net.Http;
using System.Text;

namespace Nubetico.Frontend.Services.ProyectosConstruccion
{
    public class SectionDetailsServices(IHttpClientFactory httpClientFactory)
    {
        #region INJECTION
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");
        #endregion

        #region CONST
        private const string API_URL_BASE = "api/v1/proyectosconstruccion/sections-details";
        #endregion

        #region HTTP POST
        public async Task<BaseResponseDto<PaginatedListDto<SectionDetailsDto>?>> GetSectionsPaginatedListAsync(RequestSectionDetailsCatDto request)
        {
            // Serializa el objeto a JSON
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // Envía la solicitud POST
            var response = await _httpClient.PostAsync($"{API_URL_BASE}/paginated-section", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<PaginatedListDto<SectionDetailsDto>?>?>(responseContent);
            return dataResult!;
        }
        #endregion

        #region HTTP GET
        public async Task<BaseResponseDto<ResponseSectionDetailsDto?>> GetSectionFilterByIdAsync(Guid sectionGuid)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/filter-section";
                var queryParams = new Dictionary<string, string> { ["sectionGuid"] = sectionGuid.ToString() };

                var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
                var urlWithParams = $"{endpoint}?{queryString}";

                var response = await _httpClient.GetAsync(urlWithParams);
                var responseContent = await response.Content.ReadAsStringAsync();

                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<ResponseSectionDetailsDto?>>(responseContent);
                return dataResult!;
            }
            catch (Exception ex)
            {
                return new()
                {
                    StatusCode = 501,
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        #endregion
    }
}
