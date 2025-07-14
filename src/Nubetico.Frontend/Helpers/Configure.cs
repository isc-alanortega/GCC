using Microsoft.JSInterop;
using Newtonsoft.Json;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.Core;
using Nubetico.Shared.Dto.Core;
using System.Globalization;

namespace Nubetico.Frontend.Helpers
{
    public static class Configure
    {
        public static async Task SetThemeAndLanguageAsync(IServiceProvider services)
        {
            var customizationService = services.GetRequiredService<CustomizationService>();
            var jSRuntime = services.GetRequiredService<IJSRuntime>();

            var temaResponseDto = await customizationService.GetTemaAsync();

            if (temaResponseDto?.StatusCode != 200 || temaResponseDto.Data == null)
            {
                // Aplica el tema por defecto si no hay tema o el estado de la respuesta no es 200
                await ApplyDefaultTheme(jSRuntime);
                return;
            }

            var theme = JsonConvert.DeserializeObject<ThemeConfigDto>(temaResponseDto.Data.ToString());

            await ApplyCustomTheme(jSRuntime, theme);
            await SetLanguageAsync(jSRuntime, theme);
        }

        private static async Task ApplyDefaultTheme(IJSRuntime jSRuntime)
        {
            await jSRuntime.InvokeVoidAsync("applyCssAndFavicon", "/assets/nubetico/css/default.css", "/assets/nubetico/img/favicon.png");
            await jSRuntime.InvokeVoidAsync("applyPwaManifest", "/assets/nubetico/manifest.webmanifest");

            // Set logos
            await jSRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKeys.UrlMainLogo, "/assets/nubetico/img/nubetico_horizontal_w.svg");
            await jSRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKeys.UrlLoginLogo, "/assets/nubetico/img/nubetico_horizontal_w.svg");
        }

        private static async Task ApplyCustomTheme(IJSRuntime jSRuntime, ThemeConfigDto theme)
        {
            // Guarda datos en localStorage y aplica CSS y favicon personalizados
            await jSRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKeys.WebName, theme.NombreWeb);
            await jSRuntime.InvokeVoidAsync("applyCssAndFavicon", theme.UrlCss, theme.UrlFavicon);
            await jSRuntime.InvokeVoidAsync("applyPwaManifest", theme.UrlWebManifest);
            await jSRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKeys.LanguajeEnabled, theme.CambiarIdioma);

            // Set logos
            await jSRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKeys.UrlMainLogo, theme.UrlLogoPrincipal);
            await jSRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKeys.UrlLoginLogo, theme.UrlLogoLogin);
        }

        private static async Task SetLanguageAsync(IJSRuntime jSRuntime, ThemeConfigDto theme)
        {
            // Obtiene el idioma guardado en localStorage
            var storedCulture = await jSRuntime.InvokeAsync<string>("localStorage.getItem", LocalStorageKeys.NbCulture);

            if (!string.IsNullOrWhiteSpace(storedCulture))
            {
                // Establecer la cultura si se encuentra en localStorage
                SetCulture(storedCulture);
            }
            else
            {
                // Establecer idioma por defecto del tema o español
                var cultureToSet = !string.IsNullOrEmpty(theme.IdiomaDefault) ? theme.IdiomaDefault : "es-MX";
                await jSRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKeys.NbCulture, cultureToSet);
                SetCulture(cultureToSet);
            }
        }

        private static void SetCulture(string cultureCode)
        {
            var culture = new CultureInfo(cultureCode);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

    }
}
