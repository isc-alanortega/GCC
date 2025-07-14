using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Nubetico.Frontend.Helpers;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Models.Static.Core;
using System.Globalization;

namespace Nubetico.Frontend.Components.Core.Shared
{
    public partial class CultureProvider
    {
        [Inject]
        private IJSRuntime JS { get; set; }

        private bool setEnglish = CultureInfo.CurrentCulture.Name == "en-US";
        private bool showComponent = false;

        protected override async Task OnInitializedAsync()
        {
            // Obtener el valor desde localStorage
            var value = await JS.InvokeAsync<string>("localStorage.getItem", LocalStorageKeys.LanguajeEnabled);

            // Si el valor es "mostrar", entonces se mostrará el componente
            if (!string.IsNullOrEmpty(value) && value == "true")
            {
                showComponent = true;
            }
        }

        private async Task ChangeLanguage(bool? setEnglish)
        {
            var selectedCulture = setEnglish ?? false ? "en-US" : "es-MX";

            await JS.InvokeVoidAsync("localStorage.setItem", LocalStorageKeys.NbCulture, selectedCulture);

            CultureInfo culture = new CultureInfo(selectedCulture);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // Desactivar alert al tratar de cerrar nubetico
            await FrontendHelpers.SetLeavingWarningEnabled(false, JsRuntime);

            // Reload
            await JS.InvokeVoidAsync("location.reload");
        }
    }
}
