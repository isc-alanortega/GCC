using Microsoft.JSInterop;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.Core;
using System.Net.Http.Headers;

namespace Nubetico.Frontend.Helpers
{
    public class HttpClientAuthHandler : DelegatingHandler
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly AuthStateProvider _authStateProvider;

        public HttpClientAuthHandler(IJSRuntime jsRuntime, AuthStateProvider authStateProvider)
        {
            _jsRuntime = jsRuntime;
            _authStateProvider = authStateProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated ?? false)
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", LocalStorageKeys.Jwt);
                request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
