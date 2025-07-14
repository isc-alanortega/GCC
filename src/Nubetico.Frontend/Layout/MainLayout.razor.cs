using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Nubetico.Frontend.Components.Core.Shared;
using Nubetico.Frontend.Helpers;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Shared.Dto.Core;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Layout
{
    public partial class MainLayout : IAsyncDisposable
    {
        public string UrlLogoPrincipal { get; set; } = "";
        private RadzenSidebar sidebar0 { get; set; } = new RadzenSidebar();
        private RadzenBody body0 { get; set; } = new RadzenBody();
        private HubConnection? hubConnection;
        private bool IsRendered { get; set; } = false;
        private bool LeftSidebarExpanded { get; set; } = true;
        private bool RightSidebarExpanded { get; set; } = false;
        private List<MenuUsuarioDto>? opcionesMenu = new List<MenuUsuarioDto>();
        private List<MenuUsuarioDto>? opcionesMenuBack = new List<MenuUsuarioDto>();
        public bool isAuthenticated { get; set; }

        protected override async Task OnInitializedAsync()
        {
            LeftSidebarExpanded = !await FrontendHelpers.GetIsMobile(JsRuntime);

            await FrontendHelpers.SetLeavingWarningEnabled(true, JsRuntime);

            UrlLogoPrincipal = await JsRuntime.InvokeAsync<string>("localStorage.getItem", LocalStorageKeys.UrlMainLogo);

            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (!(user.Identity?.IsAuthenticated ?? false))
            {
                return;
            }

            var result = await MenuService.GetUserMenusAsync();
            if (!result.Success || result.Data == null)
            {
                return;
            }

            opcionesMenu = JsonConvert.DeserializeObject<List<MenuUsuarioDto>>(result.Data.ToString());
            opcionesMenuBack = opcionesMenu;

            // Configurar hub de notificaciones
            hubConnection = new HubConnectionBuilder()
                .WithUrl(Navigation.ToAbsoluteUri("/notificaciones"))
                .Build();

            // Método para recibir notificaciones del servidor
            hubConnection.On<NotificacionDto>("ReceiveNotification", (notificacionDto) =>
            {
                ShowNotification(new NotificationMessage { Severity = (NotificationSeverity)notificacionDto.Severidad, Summary = notificacionDto.Titulo, Detail = notificacionDto.Mensaje, Duration = notificacionDto.DuracionMs });
                InvokeAsync(StateHasChanged);
            });

            await hubConnection.StartAsync();

            BreakpointService.OnChange += StateHasChanged;

            await BreakpointService.InitializeAsync();
        }

        public void Dispose()
        {
            BreakpointService.OnChange -= StateHasChanged;
        }

        private void ToggleSidebar()
        {
            LeftSidebarExpanded = !LeftSidebarExpanded;
            InvokeAsync(StateHasChanged);
        }

        public List<MenuUsuarioDto>? Filter(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return opcionesMenuBack;
            }

            bool contains(string value) => value != null && value.Contains(term, StringComparison.OrdinalIgnoreCase);
            bool filter(MenuUsuarioDto MenuDto) => contains(MenuDto.Name) || MenuDto.Tags != null && MenuDto.Tags.Any(contains);
            bool deepFilter(MenuUsuarioDto example) => filter(example) || example.Children?.Any(filter) == true;

            return opcionesMenuBack?.Where(category => category.Children?.Any(deepFilter) == true || filter(category))
                           .Select(category => new MenuUsuarioDto
                           {
                               Name = category.Name,
                               Path = category.Path,
                               Icon = category.Icon,
                               Expanded = true,
                               ComponentNamespace = category.ComponentNamespace,
                               ComponentTypeName = category.ComponentTypeName,
                               Children = category.Children?.Where(deepFilter).Select(example => new MenuUsuarioDto
                               {
                                   Name = example.Name,
                                   Path = example.Path,
                                   Icon = example.Icon,
                                   Expanded = true,
                                   ComponentNamespace = example.ComponentNamespace,
                                   ComponentTypeName = example.ComponentTypeName,
                                   Children = example.Children
                               }
                               ).ToArray()
                           }).ToList();
        }

        private void FilterPanelMenu(ChangeEventArgs args)
        {
            var term = args.Value?.ToString();
            opcionesMenu = Filter(term);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                IsRendered = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (opcionesMenu != null)
            {
                opcionesMenu = null;
            }

            if (opcionesMenuBack != null)
            {
                opcionesMenuBack = null;
            }
        }

        private async Task OpenSideDialog()
        {
            await DialogService.OpenSideAsync<ProfileComponent>(Localizer["Core.Profile"], options: new SideDialogOptions { CloseDialogOnOverlayClick = false, Position = DialogPosition.Right, ShowMask = true });
        }

        private void ShowNotification(NotificationMessage message)
        {
            NotificationService.Notify(message);
        }
    }
}
