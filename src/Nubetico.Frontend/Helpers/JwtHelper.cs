using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.JSInterop;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.Core;
using System.Security.Claims;
using System.Text.Json;

namespace Nubetico.Frontend.Helpers
{
    public static class JwtHelper
    {
        public static async Task ValidateJwtAndHandleExpirationAsync(IServiceProvider services)
        {
            var jsRuntime = services.GetRequiredService<IJSRuntime>();

            // Obtener jwt en localStorage
            var jwtAlmacenado = await jsRuntime.InvokeAsync<string>("localStorage.getItem", LocalStorageKeys.Jwt);

            if (!string.IsNullOrEmpty(jwtAlmacenado))
            {
                var handler = new JsonWebTokenHandler();
                JsonWebToken? jwtToken;

                try
                {
                    // Decodificar el JWT
                    jwtToken = handler.ReadJsonWebToken(jwtAlmacenado);
                }
                catch (Exception ex)
                {
                    jwtToken = null;
                }

                if (jwtToken != null)
                {
                    var expirationClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);

                    if (expirationClaim != null)
                    {
                        var expDateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim.Value)).UtcDateTime;

                        if (expDateTime < DateTime.UtcNow)
                        {
                            var authStateProvider = services.GetRequiredService<AuthStateProvider>();
                            ((AuthStateProvider)authStateProvider).NotifyUserSignOut();

                            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", LocalStorageKeys.Jwt);
                            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", LocalStorageKeys.Profile);
                            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", LocalStorageKeys.WorkWithTabs);
                        }
                    }
                }
            }
        }

        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();

            // Decodificar el payload en Base64
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);

            // Deserializar el JSON
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            // Iterar sobre cada clave-valor del diccionario y añadirlo a los claims
            foreach (var kvp in keyValuePairs)
            {
                if (kvp.Value is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
                {
                    // Si el valor es un array, como ocurre en algunos claims, recorrer cada valor del array
                    foreach (var item in jsonElement.EnumerateArray())
                    {
                        claims.Add(new Claim(kvp.Key, item.ToString()));
                    }
                }
                else
                {
                    claims.Add(new Claim(kvp.Key, kvp.Value.ToString()));
                }
            }

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
