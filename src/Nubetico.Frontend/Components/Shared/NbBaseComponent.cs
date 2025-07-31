using Microsoft.AspNetCore.Components;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.Core;
using Nubetico.Shared.Dto.Common;
using Radzen;
using Radzen.Blazor;
using System.Security.Claims;

namespace Nubetico.Frontend.Components.Shared
{
    public class NbBaseComponent : ComponentBase
    {
        // Inyeccion de dependencias
        [Inject] protected AuthStateProvider AuthStateProvider { get; set; }
        [Inject] protected ComponentService ComponentService { get; set; }
        [Inject] protected NavigationManager Navigation { get; set; }
        [Inject] protected NotificationService NotificationService { get; set; }

        // Icono por defecto al mostrar una pestania
        public string IconoBase = "f05a";

        // Almacena la lista de opciones de menu superior
        public List<RadzenMenuItem> OpcionesMenu = [];

        // Propiedad para almacenar errores generados con fluent validation
        public Dictionary<string, List<string>> FormValidationErrors = [];

        // Almacena el estado del control
        private TipoEstadoControl _estadoControl = TipoEstadoControl.Lectura;

        // Cuando el estado del control cambia, desencadenamos el evento para notificar el cambio
        public TipoEstadoControl EstadoControl
        {
            get => _estadoControl;
            set
            {
                if (_estadoControl != value)
                {
                    _estadoControl = value;
                    OnEstadoControlChanged();
                }
            }
        }

        // Propiedades capa de proteccion
        protected virtual bool AllowExternalUser => false;
        //protected bool IsInternalUser { get; private set; }
        //protected bool IsExternalUser => !IsInternalUser;
        //protected override async Task OnInitializedAsync()
        //{
        //    var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        //    var user = authState.User;

        //    var contactClaim = user.FindFirst("type-contact");
        //    var contactValue = contactClaim?.Value;

        //    IsInternalUser = contactValue == "0";

        //    if (IsExternalUser && !AllowExternalUser)
        //    {
        //        Navigation.NavigateTo("/not-authorized");
        //        // throw new UnauthorizedAccessException();
        //    }

        //    await base.OnInitializedAsync();
        //}

        protected void ReadFormValidationErrors(List<ValidationFailureDto> errores)
        {
            FormValidationErrors.Clear();

            foreach (var error in errores)
            {
                if (string.IsNullOrWhiteSpace(error.PropertyName) || string.IsNullOrWhiteSpace(error.ErrorMessage))
                    continue;

                if (!FormValidationErrors.ContainsKey(error.PropertyName))
                {
                    FormValidationErrors[error.PropertyName] = new List<string>();
                }

                FormValidationErrors[error.PropertyName].Add(error.ErrorMessage);
            }
        }

        protected void OnEstadoControlChanged()
        {
            ComponentService.SetCurrentTabState(this._estadoControl);
        }

        protected async Task<ClaimsPrincipal?> GetUsuarioAutenticadoAsync()
        {
            return await AuthStateProvider.GetAuthenticatedUserAsync();
        }

        protected void AgregarTabNubetico(TabNubetico tabNubetico)
        {
            ComponentService.AddComponenteTabNubetico(tabNubetico);
        }

        protected void SetNombreTabNubetico(string tabName)
        {
            string iconText = EstadoControl == TipoEstadoControl.Alta
                ? $"{MenuItemsFactory.MenuIconDictionary["agregar"]}"
                    : EstadoControl == TipoEstadoControl.Edicion
                        ? $"{MenuItemsFactory.MenuIconDictionary["editar"]}"
                        : IconoBase;


            ComponentService.SetCurrentTabName(tabName, iconText);
        }

        protected void CerrarTabNubetico()
        {
            ComponentService.SetCloseCurrentTab(this.EstadoControl == TipoEstadoControl.Alta || this.EstadoControl == TipoEstadoControl.Edicion);
        }

        protected void SetMenuItems(List<RadzenMenuItem> menuItems)
        {
            ComponentService.SetMenuItems(menuItems);
        }

        public void TriggerMenuUpdate()
        {
            // Configura el menú actual de este componente
            ComponentService.SetMenuItems(GetMenuItems());
        }

        /// <summary>
        /// Metodo a sobreescribir en cada comoponente hijo por sus propios menus
        /// </summary>
        /// <returns>Retorna la lista de opciones de menu</returns>
        protected virtual List<RadzenMenuItem> GetMenuItems()
        {
            return new List<RadzenMenuItem>();
        }

        protected void ShowNotification(NotificationMessage message)
        {
            NotificationService.Notify(message);
        }

        public enum TipoEstadoControl
        {
            Lectura = 0,
            Edicion = 1,
            Alta = 2
        }
    }
}
