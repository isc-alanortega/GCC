using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Nubetico.Frontend.Components.Shared
{
    public class NubeticoRadzenDataGrid<T> : Radzen.Blazor.RadzenDataGrid<T>
    {
        [Inject]
        private IStringLocalizer<SharedResources> Localizer { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            if (Localizer != null)
            {
                this.PagingSummaryFormat = Localizer["Shared.Textos.PagingSummaryFormat"];
            }

            await base.OnInitializedAsync();
        }
    }
}
