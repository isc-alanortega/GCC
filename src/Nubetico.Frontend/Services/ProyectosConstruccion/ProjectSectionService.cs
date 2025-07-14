using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Nubetico.Shared.Dto.ProyectosConstruccion.Sections;
using System.Text;

namespace Nubetico.Frontend.Services.ProyectosConstruccion
{
    public class ProjectSectionService(IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");
        private const string API_URL_BASE = "api/v1/proyectosconstruccion/proyectos-secciones";

        #region HTTP GET
        public async Task<BaseResponseDto<IEnumerable<ProjectSectionDataDto>?>> GetProjectFormDataAsync(int projectId)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/get-project-sections";
                var queryParams = new Dictionary<string, string> { ["projectGuid"] = projectId.ToString() };

                var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
                var urlWithParams = $"{endpoint}?{queryString}";

                var response = await _httpClient.GetAsync(urlWithParams);
                var responseContent = await response.Content.ReadAsStringAsync();
                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<IEnumerable<ProjectSectionDataDto>?>>(responseContent);

                return dataResult!;
            }
            catch (Exception ex) { 
                return new()
                {
                    StatusCode = 501,
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<BaseResponseDto<IEnumerable<ProjectSectionDataDto>?>> GetProjectsSectionsByProjectIdAsync(int projectId)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/get-project-sections";
                var queryParams = new Dictionary<string, string> { ["projectGuid"] = projectId.ToString() };

                var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
                var urlWithParams = $"{endpoint}?{queryString}";

                var response = await _httpClient.GetAsync(urlWithParams);
                var responseContent = await response.Content.ReadAsStringAsync();
                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<IEnumerable<ProjectSectionDataDto>?>>(responseContent);

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


        public async Task<BaseResponseDto<ProjectSectionFetcherDto?>> GetSectionFormDataAsync(int subdivisionId, int? sectionId)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/get-form-data";


                int sectionIdToString = sectionId ?? 0;
                var queryParams = new Dictionary<string, string>
                {
                    ["subdivisionId"] = subdivisionId.ToString(),
                    ["sectionId"] = sectionIdToString.ToString(),
                };

                var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
                var urlWithParams = $"{endpoint}?{queryString}";

                var response = await _httpClient.GetAsync(urlWithParams);
                var responseContent = await response.Content.ReadAsStringAsync();
                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<ProjectSectionFetcherDto?>>(responseContent);

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


        #region HTTP POST
        public async Task<BaseResponseDto<ProjectSectionDataDto>> PostAddSectionAsync(ProjectSectionDataDto? request)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/add";

                // Serializa el objeto a JSON
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Envía la solicitud POST
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<ProjectSectionDataDto>?>(responseContent);
                return dataResult!;
            }
            catch(Exception ex)
            {
                return new()
                {
                    StatusCode = 501,
                    Success = false,
                    Message = $"Error al recuperar la información"
                };
            }
            
        }
        #endregion

        #region HTTP PATCH
        public async Task<BaseResponseDto<bool?>> PathcDeletePhaseByIdAsync(int phaseId)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/delete-phase";
                var queryParams = new Dictionary<string, string> { ["projectGuid"] = phaseId.ToString() };

                var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
                var urlWithParams = $"{endpoint}?{queryString}";

                var response = await _httpClient.GetAsync(urlWithParams);
                var responseContent = await response.Content.ReadAsStringAsync();
                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<bool?>>(responseContent);

                return dataResult!;
            }
            catch (Exception ex)
            {
                return new()
                {
                    StatusCode = 501,
                    Success = false,
                    Message = "Error al recuperar la información"
                };
            }
        }
        #endregion
    }
}
