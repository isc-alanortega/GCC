namespace Nubetico.WebAPI.Application.External.Directory
{
    public partial class DirectoryApiServices
    {
        private readonly HttpClient _directoryHttpClient;

        public DirectoryApiServices(IHttpClientFactory httpClientFactory)
        {
            _directoryHttpClient = httpClientFactory.CreateClient("nb.directory");
        }
    }
}
