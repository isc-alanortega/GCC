using Microsoft.JSInterop;

namespace Nubetico.Frontend.Helpers
{
    public static class FrontendHelpers
    {
        public static async Task SetLeavingWarningEnabled(bool enabled, IJSRuntime jsRuntime)
        {
            await jsRuntime.InvokeVoidAsync("preventPageUnload", enabled);
        }

        public static async Task<bool> GetIsMobile(IJSRuntime jsRuntime)
        {
            return await jsRuntime.InvokeAsync<bool>("isMobile");
        }
    }
}
