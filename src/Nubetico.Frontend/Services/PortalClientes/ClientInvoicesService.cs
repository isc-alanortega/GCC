using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.PortalClientes;
using Nubetico.Shared.Dto.ProyectosConstruccion;

namespace Nubetico.Frontend.Services.PortalClientes
{
    public class ClientInvoicesService
    {
        private readonly HttpClient _httpClient;

        public ClientInvoicesService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<BaseResponseDto<PaginatedListDto<ExternalClientInvoices>>> GetPagedInvoicesAsync(int limit, int offset, ExternalInvoicesFilter filter)
        {
            string endpoint = "api/v1/portalclientes/facturas/externos";

            var queryParams = new Dictionary<string, string>
            {
                { "limit", limit.ToString() },
                { "offset", offset.ToString() }
            };

            if (!string.IsNullOrWhiteSpace(filter.Folio))
                queryParams.Add("Folio", Uri.EscapeDataString(filter.Folio));

            if (!string.IsNullOrWhiteSpace(filter.BusinessName))
                queryParams.Add("BusinessName", Uri.EscapeDataString(filter.BusinessName));

            if (!string.IsNullOrWhiteSpace(filter.Status))
                queryParams.Add("Status", Uri.EscapeDataString(filter.Status));

            if (filter.DateFrom.HasValue)
                queryParams.Add("DateFrom", filter.DateFrom.Value.ToString("yyyy-MM-dd"));

            if (filter.DateTo.HasValue)
                queryParams.Add("DateTo", filter.DateTo.Value.ToString("yyyy-MM-dd"));

            var queryString = string.Join("&", queryParams.Select(param => $"{param.Key}={param.Value}"));
            var urlWithParams = $"{endpoint}?{queryString}";


            var response = await _httpClient.GetAsync(urlWithParams);
            var responseContent = await response.Content.ReadAsStringAsync();

            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<PaginatedListDto<ExternalClientInvoices>>>(responseContent);

            return dataResult;
        }
    }
}
