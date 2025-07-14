using Microsoft.AspNetCore.Components;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Nubetico.Shared.Enums.Core;
using Radzen.Blazor;
using Radzen;
using Microsoft.Extensions.Localization;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Frontend.Components.Core.Shared;

namespace Nubetico.Frontend.Pages.ProyectosConstruccion
{
    public partial class ProyectosDetPage : NbBaseComponent
    {
        #region INJECTS
        [Inject] private IStringLocalizer<SharedResources>? LocalizerServices { get; set; }
        [Inject] private ProjectServices ProjectApiServices { get; set; }
        [Inject] private NotificationService NotificationService { get; set; }
        #endregion

        #region PARAMETER
        [Parameter] public ProjectDataDto? ProjectData { get; set; }
        #endregion

        #region PROPERTYS
        public bool IsSavingData { get; set; } = false;
        #endregion

        #region METHODS BUTTON LIST 
        protected override List<RadzenMenuItem> GetMenuItems()
        {
            var menusDefinidos = MenuItemsFactory.GetBaseMenuItems(LocalizerServices!);
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
                default:
                    AgregarMenuSiExiste(BaseMenuCommands.EDIT, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickEdit));
                    AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));
                    break;
            }

            return menusMostrar;
        }

        private async Task OnClickSaveAsync()
        {
            if (!ProjectApiServices.GetIsValidForm() || IsSavingData) return;

            IsSavingData = true;

            var result = await (this.EstadoControl switch
            {
                TipoEstadoControl.Alta => ProjectApiServices.PostAddProjectAsync(project: ProjectData!),
                TipoEstadoControl.Edicion => ProjectApiServices.PatchEditProjectAsync(project: ProjectData!),
                _ => Task.FromResult<BaseResponseDto<ProjectDataDto?>>(null)
            });

            if (result.StatusCode > 300)
            {
                NotifyAcces("titulo", result.Message, NotificationSeverity.Error);
                return;
            } 

            if (this.EstadoControl == TipoEstadoControl.Alta) 
            {
                ProjectData!.Folio = result.Data!.Folio;
            }

            NotifyAcces(summary: string.Empty, "Datos guardados con exito", NotificationSeverity.Success);
            
            this.EstadoControl = TipoEstadoControl.Lectura;
            this.SetNombreTabNubetico($"{LocalizerServices!["Shared.Textos.Project"]} [{this.ProjectData?.Folio}]");
            
            this.TriggerMenuUpdate();

            ProjectApiServices.NotifyStateChanged();
            ProjectApiServices.RefreshGridProjectCat();
            StateHasChanged();
            IsSavingData = false;
        }

        private void OnClickEdit()
        {
            if (this.EstadoControl == TipoEstadoControl.Edicion) 
            {
                NotifyAcces("", details: "Ya fue seleccionado", severity: NotificationSeverity.Error);
                return;
            }

            this.EstadoControl = TipoEstadoControl.Edicion;
            this.SetNombreTabNubetico($"{LocalizerServices!["Shared.Textos.Project"]} [{ProjectData!.Folio}]");
            this.TriggerMenuUpdate();

            ProjectApiServices.NotifyStateChanged();
            ProjectApiServices.RefreshGridProjectCat();
            StateHasChanged();
        }

        private void OnClickCancel()
        {
            this.EstadoControl = TipoEstadoControl.Lectura;
            SetNombreTabNubetico($"{LocalizerServices!["Shared.Textos.Project"]}  [{ProjectData!.Folio}]");
            this.TriggerMenuUpdate();
            ProjectApiServices.NotifyStateChanged();
            StateHasChanged();
        }

        private void OnClickClose() =>this.CerrarTabNubetico();
        #endregion

        #region LIFE CICLE BLAZOR
        protected override async Task OnInitializedAsync()
        {
            this.TriggerMenuUpdate();
            await base.OnInitializedAsync();
        }
        #endregion

        #region UTILS
        private void NotifyAcces(string summary, string details, NotificationSeverity severity) => NotificationService!.Notify(new()
             {
                 Severity = severity,
                 Summary = summary,
                 Detail = details
             });
        #endregion
    }
}
