using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using System.Text;

namespace Nubetico.Frontend.Services.ProyectosConstruccion
{
    public class ProjectServices(IHttpClientFactory? httpClientFactory)
    {
        #region INJECTION
        private readonly HttpClient _httpClient = httpClientFactory!.CreateClient("ApiClient");
        #endregion

        #region CONST
        private const string API_URL_BASE = "api/v1/proyectosconstruccion/proyectos";
        #endregion

        #region PROPERTYS
        public bool IsProjectCatIsOpen { get; set; } = false;        
        #endregion

        #region ACTIONS
        public event Action? RefreshGrid;

        public event Action? RefreshProjectCat;

        public event Func<bool>? ValidateForm;

        #endregion

        #region EVENTS INVOKE
        public void NotifyStateChanged() => RefreshGrid?.Invoke();

        public void RefreshGridProjectCat()
        {
            if(!IsProjectCatIsOpen) return;

            RefreshProjectCat?.Invoke();
        }

        public bool GetIsValidForm() => ValidateForm!.Invoke();
        #endregion

        public BaseResponseDto<T?> GetDefaultErrorParseJson<T>(Exception ex) => new BaseResponseDto<T?>
        {
            StatusCode = 501,
            Success = false,
            Message = "Shared.Core.UnknowError",
            Data = default
        };

        #region PROJECT API DATA ACCES       
        #region HTTP GET
        public async Task<BaseResponseDto<PaginatedListDto<ProyectsGridDto>?>> GetPaginatedProjectsAsync(ProjectsPaginatedRequestDto request)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/paginado";

                // Serializa el objeto a JSON
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Envía la solicitud POST
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<PaginatedListDto<ProyectsGridDto>?>>(responseContent);
                return dataResult!;
            }
            catch (Exception ex) {
                return GetDefaultErrorParseJson<PaginatedListDto<ProyectsGridDto>?>(ex);
            }
            
        }

        public async Task<BaseResponseDto<ProjectFormDataDto?>> GetProjectFormDataAsync()
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/form";

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<ProjectFormDataDto>>(responseContent);
                return dataResult!;
            }
            catch (Exception ex)
            {
                return GetDefaultErrorParseJson<ProjectFormDataDto?>(ex);
            }
        }

        public async Task<BaseResponseDto<ProjectDataDto?>> GetFilterProjectAsync(Guid projectGuid)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/filter";
                var queryParams = new Dictionary<string, string> { [nameof(projectGuid)] = projectGuid.ToString() };

                var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
                var urlWithParams = $"{endpoint}?{queryString}";

                var response = await _httpClient.GetAsync(urlWithParams);
                var responseContent = await response.Content.ReadAsStringAsync();
                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<ProjectDataDto?>>(responseContent);

                return dataResult!;
            }catch (Exception ex)
            {
                return GetDefaultErrorParseJson<ProjectDataDto?>(ex);
            }
            
        }
        #endregion

        #region HTTP POST
        public async Task<BaseResponseDto<ProjectDataDto?>> PostAddProjectAsync(ProjectDataDto project)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/add";

                // Serializa el objeto a JSON
                var json = JsonConvert.SerializeObject(project);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Envía la solicitud POST
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<ProjectDataDto?>?>(responseContent);
                return dataResult!;
            }
            catch (Exception ex)
            {
                return GetDefaultErrorParseJson<ProjectDataDto?>(ex);
            }
        }
        #endregion

        #region HTTP PATCH
        public async Task<BaseResponseDto<ProjectDataDto?>> PatchEditProjectAsync(ProjectDataDto project)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/update";

                // Serializa el objeto a JSON
                var json = JsonConvert.SerializeObject(project);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Envía la solicitud PATCH
                var response = await _httpClient.PatchAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<ProjectDataDto?>?>(responseContent);
                return dataResult!;
            }
            catch(Exception ex)
            {
                return GetDefaultErrorParseJson<ProjectDataDto?>(ex);
            }
        }

        public async Task<BaseResponseDto<bool?>> PatchDeleteProjectAsync(int projectId)
        {
            try
            {
                string endpoint = $"{API_URL_BASE}/delte";
                var queryParams = new Dictionary<string, string> { ["projectId"] = projectId.ToString() };

                var queryString = string.Join("&", queryParams.Select(parameter => $"{parameter.Key}={parameter.Value}"));
                var urlWithParams = $"{endpoint}?{queryString}";

                var response = await _httpClient.GetAsync(urlWithParams);
                var responseContent = await response.Content.ReadAsStringAsync();

                var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<bool?>>(responseContent);
                return dataResult!;
            }
            catch (Exception ex)
            {
                return GetDefaultErrorParseJson<bool?>(ex);
            }
        }
        #endregion

        #endregion
    }
}
