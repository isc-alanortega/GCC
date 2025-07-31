using Microsoft.AspNetCore.Components;

namespace Nubetico.Frontend.Helpers
{
    public class HttpClientTenantHandler : DelegatingHandler
    {
        private readonly NavigationManager _navigationManager;

        public HttpClientTenantHandler(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uri = new Uri(_navigationManager.Uri);
            string hostActual = uri.IsDefaultPort ? uri.Host : $"{uri.Host}:{uri.Port}";

            // Aqui se puede intercambiar la url en el header para probar diferentes diseños
            //request.Headers.Add("X-SITE", "localhost:5241");
            //request.Headers.Add("X-SITE", "gcc.dev.nubetico.com");
            //request.Headers.Add("X-SITE", "pt-destajos.nubetico.com");

            request.Headers.Add("X-SITE", hostActual);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
