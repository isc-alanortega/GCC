using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Radzen;
using Radzen.Blazor;
using System.Globalization;

namespace Nubetico.Frontend.Components.Core
{
    public partial class UsuariosDetComponent : NbBaseComponent
    {
        [Parameter]
        public string? GuidUsuario { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IStringLocalizer<SharedResources> Localizer { get; set; }

        [Inject]
        protected UsuariosService UsuariosService { get; set; }

        [Inject]
        protected MenuService MenuService { get; set; }

        [Inject]
        protected SucursalesService SucursalesService { get; set; }

        private UsuarioNubeticoDto? UsuarioDto { get; set; } = new UsuarioNubeticoDto();
        private IEnumerable<MenuDto> SelectMenus { get; set; } = [];
        private IEnumerable<object> CheckedMenuDtoValues { get; set; } = [];

        private List<SucursalDto> SucursalesList { get; set; } = new List<SucursalDto>();
        private IEnumerable<MenuPermisosDto> SelectMenuPermisos { get; set; } = [];
        private object? MenuPermisoSelected { get; set; }
        private object? SucursalPermisosSeleccionada { get; set; }
        private object? SucursalSeleccionada { get; set; }

        public bool CuentaEsValidada { get; set; } = false;
        public string CuentaValidada { get; set; } = string.Empty;
        public bool PermitirCapturaPersonales { get; set; } = false;

        private readonly string IconoBuscar = "\uf002";
        private readonly string IconoValido = "\uf00c";

        protected override async Task OnInitializedAsync()
        {
            await RefreshUsuarioAsync();
            await LoadMenusPermisosAsync();

            await base.OnInitializedAsync();
        }

        private async Task LoadMenusPermisosAsync()
        {
            var menuPermisosResponse = await MenuService.GetAllMenuWithPermissionsAsync();
            if (menuPermisosResponse.Success)
            {
                this.SelectMenuPermisos = JsonConvert.DeserializeObject<List<MenuPermisosDto>>(menuPermisosResponse.Data.ToString());
            }

            var sucursalesResponse = await SucursalesService.GetSucursalesAsync();
            if(sucursalesResponse.Success)
            {
                var sucursalesApi = JsonConvert.DeserializeObject<List<SucursalDto>>(sucursalesResponse.Data.ToString());

                var currentCulture = CultureInfo.CurrentCulture.Name;
                this.SucursalesList.Add(new SucursalDto { IdSucursal = -1, Descripcion = currentCulture == "en-US" ? "ALL" : "TODAS" });
                this.SucursalesList.AddRange(sucursalesApi);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && EstadoControl == TipoEstadoControl.Alta)
            {
                this.CuentaEsValidada = false;
                var allMenusResponse = await MenuService.GetAllMenusAsync();
                if (allMenusResponse.Success)
                {
                    this.SelectMenus = JsonConvert.DeserializeObject<List<MenuDto>>(allMenusResponse.Data.ToString());
                }
            }
        }

        // Esta función se hace la sobrescritura a la función heredada de NbBaseComponent
        protected override List<RadzenMenuItem> GetMenuItems()
        {
            var menusDefinidos = MenuItemsFactory.GetBaseMenuItems(Localizer);
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

            if (this.EstadoControl == TipoEstadoControl.Alta)
            {
                AgregarMenuSiExiste(BaseMenuCommands.SAVE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnGuardarClickAsync));
                AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnCerrarClick));
            }
            else if (this.EstadoControl == TipoEstadoControl.Edicion)
            {
                AgregarMenuSiExiste(BaseMenuCommands.SAVE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnGuardarClickAsync));
                AgregarMenuSiExiste(BaseMenuCommands.CANCEL, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnCancelarClickAsync));
                AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnCerrarClick));
            }
            else if (this.EstadoControl == TipoEstadoControl.Lectura)
            {
                AgregarMenuSiExiste(BaseMenuCommands.EDIT, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnEditarClick));
                AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnCerrarClick));
            }

            return menusMostrar;
        }

        public async Task RefreshUsuarioAsync()
        {
            if (string.IsNullOrEmpty(GuidUsuario))
            {
                this.UsuarioDto = new UsuarioNubeticoDto();
                return;
            }

            var usuarioGetResponse = await UsuariosService.GetUsuarioByGuidAsync(GuidUsuario);

            if (usuarioGetResponse != null)
            {
                if (usuarioGetResponse.Success != true)
                    return;

                this.UsuarioDto = JsonConvert.DeserializeObject<UsuarioNubeticoDto>(usuarioGetResponse.Data.ToString());

                // RadzenTree requiere que se le proporcione los menus a renderizar
                this.SelectMenus = this.UsuarioDto.ListaMenus;

                // y en esta otra propiedad los que ya están seleccionados usando como base la misma lista completa
                this.CheckedMenuDtoValues = RecursiveMenuDtoFilter(this.UsuarioDto.ListaMenus).ToList();
            }
        }

        /// <summary>
        /// Itera de forma recursiva los nodos del menu y devuelve aquellos que la propiedad Check = true o que la totalidad de los nodos tengan chek = true si seleccionable = false
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public IEnumerable<MenuDto> RecursiveMenuDtoFilter(IEnumerable<MenuDto> menus)
        {
            return menus
                .Where(menu =>
                    (menu.Check && menu.Seleccionable)
                    || (!menu.Seleccionable && GetMenuDtoChildrenIsCheck(menu))
                )
                .Concat(
                    menus
                    .Where(menu => menu.Children != null && menu.Children.Any())
                    .SelectMany(menu => RecursiveMenuDtoFilter(menu.Children))
                );
        }

        /// <summary>
        /// Valida si todos los nodos hijos de MenuDto tienen propiedad Check = true
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        private bool GetMenuDtoChildrenIsCheck(MenuDto menu)
        {
            if (menu.Children == null || !menu.Children.Any())
                return false;

            return menu.Children.All(child =>
                (child.Check || !child.Seleccionable && GetMenuDtoChildrenIsCheck(child))
            );
        }

        private void OnEditarClick(MouseEventArgs args)
        {
            this.EstadoControl = TipoEstadoControl.Edicion;
            this.SetNombreTabNubetico($"Usuario [{this.UsuarioDto?.Username}]");
            this.TriggerMenuUpdate();
        }

        private void OnCerrarClick(MouseEventArgs args)
        {
            this.CerrarTabNubetico();
        }

        protected async Task OnVerifiedAccountChanged()
        {
            this.CuentaEsValidada = CuentaValidada == UsuarioDto.Username;
        }

        private async Task OnGuardarClickAsync(MouseEventArgs args)
        {
            if (EstadoControl == TipoEstadoControl.Alta && CuentaEsValidada == false)
            {
                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Localizer["Core.Users.ErrorVerifyAccount"], Duration = 10000 });
                return;
            }

            bool? dialogResult = await DialogService.Confirm(
                Localizer["Shared.Dialog.SaveChanges"],
                Localizer["Shared.Dialog.Atencion"],
                new ConfirmOptions
                {
                    OkButtonText = Localizer["Shared.Botones.Aceptar"],
                    CancelButtonText = Localizer["Shared.Botones.Cancelar"]
                }
            );

            if (dialogResult != true) return;

            // Recuperar Menus seleccionados
            UsuarioDto.ListaMenus = CheckedMenuDtoValues.Cast<MenuDto>();

            var response = this.EstadoControl == TipoEstadoControl.Alta
                    ? await UsuariosService.PostUsuarioAsync(UsuarioDto)
                    : await UsuariosService.PutUsuarioAsync(UsuarioDto);

            if (!response.Success)
            {
                // 400: Validación del cliente
                if (response.StatusCode == 400)
                {
                    var errores = JsonConvert.DeserializeObject<List<ValidationFailureDto>>(response.Data.ToString());
                    if (errores != null)
                        ReadFormValidationErrors(errores);

                    return;
                }

                // 500: Error en servidor
                if (response.StatusCode == 500)
                {
                    ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = response.Message, Duration = 10000 });
                    return;
                }

                return;
            }

            UsuarioDto = JsonConvert.DeserializeObject<UsuarioNubeticoDto>(response.Data.ToString());
            this.GuidUsuario = UsuarioDto.UUID.Value.ToString();
            this.EstadoControl = TipoEstadoControl.Lectura;
            this.SetNombreTabNubetico($"{Localizer["Core.Users.User"]} [{this.UsuarioDto?.Username}]");
            this.TriggerMenuUpdate();

            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = Localizer["Shared.Textos.Exito"], Detail = Localizer["Shared.Textos.RegistroGuardado"], Duration = 10000 });
        }

        private async Task OnCancelarClickAsync(MouseEventArgs args)
        {
            await this.RefreshUsuarioAsync();

            this.EstadoControl = TipoEstadoControl.Lectura;
            this.SetNombreTabNubetico($"Usuario [{this.UsuarioDto?.Username}]");
            this.TriggerMenuUpdate();
        }

        private async Task OnClickValidarUsuarioAsync(MouseEventArgs args)
        {
            CuentaEsValidada = false;
            UsuarioDto.Username = UsuarioDto.Username.Trim();

            if (string.IsNullOrEmpty(UsuarioDto.Username))
                return;

            var result = await UsuariosService.GetUsuarioExisteAsync(UsuarioDto.Username);

            if (result == null)
            {
                PermitirCapturaPersonales = true;
                CuentaValidada = UsuarioDto.Username;
                return;
            }

            if (!result.Success)
            {
                CuentaEsValidada = true;
                CuentaValidada = UsuarioDto.Username;
                PermitirCapturaPersonales = true;
                UsuarioDto.Email = UsuarioDto.Username;
                return;
            }

            var usuarioExiste = JsonConvert.DeserializeObject<UsuarioExisteDirectoryDto>(result.Data.ToString());

            if (usuarioExiste.EnTenant && usuarioExiste.Confirmado)
            {

                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Localizer["Core.Users.ErrorUserAccountExists"].ToString().Replace("__ACCOUNT__", UsuarioDto.Username), Duration = 10000 });
                return;
            }

            if (usuarioExiste.EnTenant && !usuarioExiste.Confirmado)
            {
                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Localizer["Core.Users.ErrorUserAccountWait"].ToString().Replace("__ACCOUNT__", UsuarioDto.Username), Duration = 10000 });
                return;
            }

            UsuarioDto.Username = usuarioExiste.Username;
            UsuarioDto.Nombres = usuarioExiste.Nombres;
            UsuarioDto.PrimerApellido = usuarioExiste.PrimerApellido;
            UsuarioDto.SegundoApellido = usuarioExiste.SegundoApellido;
            UsuarioDto.Email = usuarioExiste.Email;
            CuentaEsValidada = true;
            CuentaValidada = UsuarioDto.Username;
            PermitirCapturaPersonales = false;
        }

        private async Task Delete2faCodeAsync()
        {
            bool? dialogResult = await DialogService.Confirm(
                    Localizer["Core.Users.AskErase2fa"],
                    Localizer["Shared.Dialog.Atencion"],
                    new ConfirmOptions
                    {
                        OkButtonText = Localizer["Shared.Botones.Aceptar"],
                        CancelButtonText = Localizer["Shared.Botones.Cancelar"]
                    }
                );

            if (dialogResult != true) return;

            UsuarioDto.TwoFactorKey = null;

            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = Localizer["Shared.Textos.Exito"], Detail = Localizer["Core.Users.Erased2faSaveToConfirm"], Duration = 10000 });
        }

        private void AsignarPermiso(PermisoDto permiso)
        {
            SucursalDto? sucursal = SucursalPermisosSeleccionada as SucursalDto;

            if(sucursal == null)
            {
                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Localizer["Core.Users.ErrorSelectBranchPermission"], Duration = 10000 });
                return;
            }

            MenuPermisosDto? menuPermisoSelect = (MenuPermisosDto?)MenuPermisoSelected;

            bool? permisoExistente = this.UsuarioDto?.Permisos.Where(m => m.IdPermiso == permiso.IdPermiso && m.IdSucursal == sucursal.IdSucursal).Any();
            if (permisoExistente ?? false)
            {
                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Localizer["Core.Users.ErrorPermissionAlreadyAssigned"], Duration = 10000 });
                return;
            }

            if (menuPermisoSelect != null)
                this.UsuarioDto?.Permisos.Add(new PermisoAsignadoDto { IdMenu = menuPermisoSelect.IdMenu, IdPermiso = permiso.IdPermiso, Permiso = permiso.Descripcion, IdSucursal = sucursal.IdSucursal, Sucursal = sucursal.Descripcion });
        }

        private void QuitarPermiso(int idPermiso, int idSucursal)
        {
            this.UsuarioDto?.Permisos.RemoveAll(m => m.IdPermiso == idPermiso && m.IdSucursal == idSucursal);
        }

        private void AsignarSucursal()
        {
            SucursalDto? sucursal = SucursalSeleccionada as SucursalDto;

            if (sucursal == null)
            {
                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Localizer["Core.Users.ErrorSelectBranch"], Duration = 10000 });
                return;
            }

            bool? permisoExistente = this.UsuarioDto?.Sucursales.Where(m => m.IdSucursal == sucursal.IdSucursal).Any();
            if (permisoExistente ?? false)
            {
                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = Localizer["Core.Users.ErrorBranchAlreadyAssigned"], Duration = 10000 });
                return;
            }

            this.UsuarioDto?.Sucursales.Add(sucursal);
        }

        private void QuitarSucursal(int idSucursal)
        {
            this.UsuarioDto?.Sucursales.RemoveAll(m => m.IdSucursal == idSucursal);
        }
    }
}
