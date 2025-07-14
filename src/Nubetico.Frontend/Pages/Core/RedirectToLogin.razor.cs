using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Nubetico.Frontend.Pages.Core
{
    public partial class RedirectToLogin
    {
        [Inject]
        private NavigationManager NavManager { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthState { get; set; }
        public bool NotAuthorized { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthState;

            if (authState.User == null)
            {
                var returnUrl = NavManager.ToBaseRelativePath(NavManager.Uri);
                if (string.IsNullOrEmpty(returnUrl))
                {
                    NavManager.NavigateTo("Login", true);
                }
                else
                {
                    NavManager.NavigateTo($"Login?returnUrl={returnUrl}", true);
                }
            }
            else
                NotAuthorized = true;
        }
    }
}
