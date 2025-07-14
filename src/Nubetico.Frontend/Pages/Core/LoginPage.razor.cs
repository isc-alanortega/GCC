using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Nubetico.Frontend.Helpers;
using Nubetico.Frontend.Models.Enums.Core;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.Core;
using Nubetico.Shared.Dto.Core;

namespace Nubetico.Frontend.Pages.Core
{
    public partial class LoginPage
    {
        [Inject]
        protected AuthService AuthService { get; set; }

        [Inject]
        protected AuthStateProvider AuthStateProvider { get; set; }

        [Inject]
        protected HttpClient HttpClient { get; set; }

        [Inject]
        protected IStringLocalizer<SharedResources> Localizer { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        private AuthRequestDto model { get; set; } = new AuthRequestDto { Username = "", Password = "", Token = "" };

        private PasosAutenticacion PasoActual { get; set; } = PasosAutenticacion.Credenciales;
        private bool TokenRequerido { get; set; } = false;
        private string Message { get; set; } = "";

        protected override async Task OnInitializedAsync()
        {
            await FrontendHelpers.SetLeavingWarningEnabled(false, JsRuntime);
        }

        private async Task ExecuteAsync()
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                this.Message = Localizer["Core.Login.ErrorCredentialsEmpty"];
                return;
            }

            var result = await AuthService.GetAutenticacion(model);

            if (!result.Success || result.Data == null)
            {
                this.Message = result.Message;
                this.PasoActual = PasosAutenticacion.Credenciales;
                return;
            }

            var authResponseDto = JsonConvert.DeserializeObject<AuthResponseDto>(result.Data.ToString());

            if (authResponseDto.TwoFactorRequired && string.IsNullOrEmpty(model.Token))
            {
                this.PasoActual = PasosAutenticacion.Token;
                Message = Localizer["Core.Login.ErrorTokenEmpty"];
                return;
            }

            if (authResponseDto.TwoFactorRequired && !string.IsNullOrEmpty(model.Token))
            {
                this.Message = result.Message;
                return;
            }

            await JsRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKeys.Profile, JsonConvert.SerializeObject(authResponseDto.PerfilUsuario));
            await JsRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKeys.Jwt, authResponseDto.JwtData.Token);
            await JsRuntime.InvokeVoidAsync("localStorage.setItem", LocalStorageKeys.WorkWithTabs, authResponseDto.PerfilUsuario.NavegaTabs);

            ((AuthStateProvider)AuthStateProvider).NotifyUserSignIn(authResponseDto.JwtData.Token);

            NavigationManager.NavigateTo("/");
        }

        private void ResetForm()
        {
            this.model.Token = "";
            this.Message = "";
            this.PasoActual = PasosAutenticacion.Credenciales;
        }
    }
}
