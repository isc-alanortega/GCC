using Microsoft.JSInterop;
using Nubetico.Frontend.Models.Static.Core;

namespace Nubetico.Frontend.Helpers
{
    public class HttpClientLanguageHandler : DelegatingHandler
    {
        private readonly IJSRuntime _jSRuntime;

        public HttpClientLanguageHandler(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var language = await _jSRuntime.InvokeAsync<string>("localStorage.getItem", LocalStorageKeys.NbCulture);

            if (!string.IsNullOrEmpty(language))
            {
                request.Headers.AcceptLanguage.Clear();
                request.Headers.AcceptLanguage.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue(language));
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
