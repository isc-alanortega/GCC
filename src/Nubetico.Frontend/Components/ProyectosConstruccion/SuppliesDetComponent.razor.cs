using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Nubetico.Frontend.Models.Enums.ProyectosCostruccion;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proyecto;
using Nubetico.Shared.Dto.ProyectosConstruccion.Supplies;
using Radzen.Blazor;
using Radzen;
using Nubetico.Frontend.Components.Core.Shared;
using Nubetico.Shared.Dto.Common;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class SuppliesDetComponent : NbBaseComponent
    {
        #region INJETIONS
        [Inject] SuppliesService SuppliesDA {  get; set; }
        [Inject] private IStringLocalizer<SharedResources>? Localizer { get; set; }
        [Inject] private NotificationService NotificationService { get; set; }
        #endregion

        #region FORM
        private RadzenTemplateForm<SuppliesDto> SupplyForm;
        #endregion

        #region PARAMETER
        [Parameter] public SuppliesDto SupplyData { get; set; }
        #endregion

        #region PROPERTYS
        public List<GroupsCatalogDto> TypesSupplies { get; set; } = [];
        public List<GroupsCatalogDto> UmSupplies { get; set; } = [];
        private bool IsSaving {  get; set; } = false;
        #endregion

        #region LIFE CICLE BLAZR
        protected override async Task OnInitializedAsync()
        {
            var usuario = await GetUsuarioAutenticadoAsync();
            SupplyData!.ActionUserGuid = Guid.TryParse(usuario?.FindFirst("id")?.Value, out Guid guid) ? guid : null;

            this.TriggerMenuUpdate();
            await GetFetchForm();
            await base.OnInitializedAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firsRender)
        {
            if (firsRender && this.EstadoControl == TipoEstadoControl.Alta)
                IsValidForm();
        }
        #endregion

        #region Form
        private async Task GetFetchForm()
        {
            var response = await SuppliesDA.GetFetcherForm();
            if (response == null || !response.Success || response.Data == null) return;
            
            TypesSupplies = response!.Data!.TypesSupplies.ToList();
            UmSupplies = response!.Data!.UmSupplies.ToList();
        }

        private bool IsDisabledForm() => this.EstadoControl == TipoEstadoControl.Lectura;
                       
        private bool IsValidForm() => SupplyForm.EditContext.Validate();

        private async Task HandleSaveAsync()
        {
            var result = await (this.EstadoControl switch
            {
                TipoEstadoControl.Alta => SuppliesDA.PostAddSupplyAsync(supply: SupplyData!),
                TipoEstadoControl.Edicion => SuppliesDA.PatchEditSupplyAsync(supply: SupplyData!),
                _ => Task.FromResult<BaseResponseDto<SuppliesDto?>>(null)
            });

            if (result.StatusCode > 300)
            {
                var message = result.StatusCode > 400 ? Localizer!["Shared.Text.UnknowError"] : result.Message;
                NotifyAcces(Localizer!["Shared.Text.ProblemOcurred"], message, NotificationSeverity.Error);
                return;
            }

            NotifyAcces(string.Empty, Localizer!["Shared.Text.SaveSucces"], NotificationSeverity.Success);

            UpdateTab(state: TipoEstadoControl.Lectura);
        }
        #endregion

        #region UTILS
        private void NotifyAcces(string summary, string details, NotificationSeverity severity) => NotificationService!.Notify(new()
        {
            Severity = severity,
            Summary = summary,
            Detail = details
        });

        #region TAB HEADER
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
                    AgregarMenuSiExiste(BaseMenuCommands.SAVE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickSave));
                    AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));
                    break;
                case TipoEstadoControl.Edicion:
                    AgregarMenuSiExiste(BaseMenuCommands.SAVE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickSave));
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

        private async void OnClickSave()
        {
            if (!IsValidForm() || IsSaving) return;

            IsSaving = true;

            await HandleSaveAsync();

            //SuppliesDA.NotifyStateChanged();
            StateHasChanged();
            IsSaving = false;
        }       

        private void OnClickEdit()
        {
            if (this.EstadoControl == TipoEstadoControl.Edicion) return;

            UpdateTab(state: TipoEstadoControl.Edicion);
        }

        private async void OnClickCancel()
        {
            UpdateTab(state: TipoEstadoControl.Lectura);

            if (this.EstadoControl != TipoEstadoControl.Edicion) return;

            var response = await SuppliesDA.GetSuppliesById(SupplyData.SuppliesId!.Value);
            if (!response!.Success || response.Data is null) return;

            SupplyData = response.Data;
        }

        private void UpdateTab(TipoEstadoControl state)
        {
            this.EstadoControl = state;
            SetNombreTabNubetico($"{Localizer!["Shared.Text.Supply"]} [{SupplyData!.Code}]");
            this.TriggerMenuUpdate();
            StateHasChanged();
        }

        private void OnClickClose() => this.CerrarTabNubetico();
        #endregion
        #endregion
    }
}
