namespace Nubetico.WebAPI.Application.External.Directory
{
    public partial class DirectoryApiServices
    {
        public async Task<HttpResponseMessage?> GetTenantsByInstanciaAsync(string instanciaUUID)
        {
            var response = await _directoryHttpClient.GetAsync($"{DirectoryEndpoints.TenantsPorInstancia}/{instanciaUUID}");
            return response;
        }
    }
}
