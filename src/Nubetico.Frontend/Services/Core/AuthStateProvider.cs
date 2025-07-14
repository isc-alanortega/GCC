using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Nubetico.Frontend.Helpers;
using Nubetico.Frontend.Models.Static.Core;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Nubetico.Frontend.Services.Core
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public AuthStateProvider(HttpClient httpClient, IJSRuntime JsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = JsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", LocalStorageKeys.Jwt);

            if (token == null)
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtHelper.ParseClaimsFromJwt(token), "jwtAuthType")));
        }

        public void NotifyUserSignIn(string token)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(JwtHelper.ParseClaimsFromJwt(token), "jwtAuthType"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserSignOut()
        {
            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task<ClaimsPrincipal?> GetAuthenticatedUserAsync()
        {
            var authState = await GetAuthenticationStateAsync();
            return authState.User;
        }
    }
}
