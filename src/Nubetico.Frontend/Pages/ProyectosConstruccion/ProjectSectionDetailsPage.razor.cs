using Microsoft.AspNetCore.Components;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Radzen.Blazor;
using Radzen;
using Microsoft.Extensions.Localization;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Frontend.Components.Core.Shared;
using Nubetico.Shared.Dto.ProyectosConstruccion.ProjectSectionDetails;

namespace Nubetico.Frontend.Pages.ProyectosConstruccion
{
    public partial class ProjectSectionDetailsPage : NbBaseComponent
    {
        #region INJECTS
        [Inject] private IStringLocalizer<SharedResources>? Localizer { get; set; }
        [Inject] private NotificationService Notification { get; set; }
        #endregion

        #region PARAMETERS
        [Parameter] public SectionDetailsDto SectionDetails { get; set; }
        [Parameter] public List<SectionLotsDto> SectionLots { get; set; }
        #endregion

        protected override void OnInitialized()
        {
            this.TriggerMenuUpdate();
            base.OnInitialized();
        }

        #region METHODS BUTTON LIST 
        protected override List<RadzenMenuItem> GetMenuItems()
        {
            var menusDefinidos = MenuItemsFactory.GetBaseMenuItems(Localizer!);
            var menusMostrar = new List<RadzenMenuItem>();

            void AgregarMenuSiExiste(string comando, EventCallback<MenuItemEventArgs> onClick)
            {
                var menu = menusDefinidos.FirstOrDefault(m => m.Attributes != null
                    && m.Attributes.TryGetValue("comando", out var comandoValue)
                    && comandoValue.ToString() == comando);

                if (menu != null)
                {
                    menu.Click = onClick;
                    menusMostrar.Add(menu);
                }
            }

            switch (this.EstadoControl)
            {
                case TipoEstadoControl.Alta:
                    AgregarMenuSiExiste(BaseMenuCommands.SAVE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickSaveAsync));
                    AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));
                    break;
                case TipoEstadoControl.Edicion:
                    AgregarMenuSiExiste(BaseMenuCommands.SAVE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickSaveAsync));
                    AgregarMenuSiExiste(BaseMenuCommands.CANCEL, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickCancel));
                    AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));
                    break;
                case TipoEstadoControl.Lectura:
                    //AgregarMenuSiExiste(BaseMenuCommands.EDIT, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickEdit));
                    AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));
                    break;
            }

            return menusMostrar;
        }

        private async Task OnClickSaveAsync()
        {
            
        }

        private void OnClickEdit()
        {
            
        }

        private void OnClickCancel()
        {
            this.EstadoControl = TipoEstadoControl.Lectura;
            SetNombreTabNubetico($"Sección [{SectionDetails.Section}]");
            this.TriggerMenuUpdate();
            StateHasChanged();
        }

        private void OnClickClose() => this.CerrarTabNubetico();
        #endregion
    }
}
