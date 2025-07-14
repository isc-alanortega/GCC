using Microsoft.JSInterop;
using Newtonsoft.Json;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.Core;
using Nubetico.Shared.Dto.Core;

namespace Nubetico.Frontend.Components.Core.Shared
{
    public partial class ProfileComponent
    {
        private PerfilUsuarioDto? Perfil { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                string jsonPerfil = await JsRuntime.InvokeAsync<string>("localStorage.getItem", LocalStorageKeys.Profile);
                Perfil = JsonConvert.DeserializeObject<PerfilUsuarioDto>(jsonPerfil);

                if (Perfil == null)
                {
                    //_navigationManager.NavigateTo("error");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el perfil: {ex.Message}");
            }
        }

        private async Task SignOutAsync()
        {
            await JsRuntime.InvokeVoidAsync("localStorage.removeItem", LocalStorageKeys.Jwt);
            await JsRuntime.InvokeVoidAsync("localStorage.removeItem", LocalStorageKeys.Profile);
            await JsRuntime.InvokeVoidAsync("localStorage.removeItem", LocalStorageKeys.WorkWithTabs);

            HttpClient.DefaultRequestHeaders.Authorization = null;
            ((AuthStateProvider)AuthStateProvider).NotifyUserSignOut();

            NavigationManager.NavigateTo("/login");
        }
    }

}
