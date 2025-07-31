using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.Core
{
    public partial class UsuariosCatComponent : NbBaseComponent
    {
        [Inject]
        protected UsuariosService UsuariosService { get; set; }

        [Inject]
        protected IStringLocalizer<SharedResources> Localizer { get; set; }

        private RadzenDataGrid<UsuarioNubeticoGridDto>? GridUsuarios { get; set; }
        private List<UsuarioNubeticoGridDto>? ListaUsuarios { get; set; }
        private int Count { get; set; }
        private bool IsLoading { get; set; } = false;
        private int RowsPerPage { get; set; } = 10;
        public IList<UsuarioNubeticoGridDto> UsuariosSeleccionados { get; set; } = new List<UsuarioNubeticoGridDto>();
        private FiltroUsuariosNubeticoGridDto Filtro { get; set; } = new FiltroUsuariosNubeticoGridDto();

        private List<BasicItemSelectDto> SelectEstadosUsuario = new List<BasicItemSelectDto>();
        public bool busy { get; set; } = false;
        protected override async Task OnInitializedAsync()
        {
            TriggerMenuUpdate();

            BaseResponseDto<object>? listRequest = await UsuariosService.GetUserStatusListAsync();
            if (listRequest.Success)
                SelectEstadosUsuario = JsonConvert.DeserializeObject<List<BasicItemSelectDto>>(listRequest.Data.ToString());
        }

        protected override List<RadzenMenuItem> GetMenuItems()
        {
            var baseMenu = MenuItemsFactory.GetBaseMenuItems(Localizer);
            var displayedMenu = new List<RadzenMenuItem>();

            void AddMenu(string comando, EventCallback<MenuItemEventArgs> onClick)
            {
                var menu = baseMenu.FirstOrDefault(m => m.Attributes != null
                    && m.Attributes.TryGetValue("comando", out var comandoValue)
                    && comandoValue.ToString() == comando);

                if (menu != null)
                {
                    menu.Click = onClick;
                    displayedMenu.Add(menu);
                }
            }

            AddMenu(BaseMenuCommands.OPEN, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnAbrirClick));
            AddMenu(BaseMenuCommands.ADD, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnAgregarClick));
            AddMenu(BaseMenuCommands.EDIT, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnEditarClick));
            AddMenu(BaseMenuCommands.REFRESH, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnRefrescarClickAsync));
            AddMenu(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnCerrarClick));

            return displayedMenu;
        }

        private void OnAgregarClick(MouseEventArgs args)
        {
            string? guidUsuario = null;

            // Crear instancia TabNubetico
            TabNubetico tabNubetico = new TabNubetico
            {
                EstadoControl = TipoEstadoControl.Alta,
                Icono = $"{MenuItemsFactory.MenuIconDictionary["agregar"]}",
                Text = Localizer["Core.Users.AddUser"],
                TipoControl = typeof(UsuariosDetComponent),
                Repetir = true
            };

            // Instanciar componente contenido en TabNubetico
            tabNubetico.Componente = builder =>
            {
                builder.OpenComponent(0, tabNubetico.TipoControl);
                builder.AddAttribute(1, "GuidUsuario", guidUsuario);
                builder.AddComponentReferenceCapture(1, instance =>
                {
                    // Asegurarnos que el componente interno instanciado hereda el componente base
                    if (instance is NbBaseComponent nbComponent)
                    {
                        tabNubetico.InstanciaComponente = nbComponent;
                        // Establecer el menú inicial para el componente
                        nbComponent.EstadoControl = TipoEstadoControl.Alta;

                        if (!string.IsNullOrEmpty(this.IconoBase))
                            nbComponent.IconoBase = this.IconoBase;

                        nbComponent.TriggerMenuUpdate();
                    }
                });
                builder.CloseComponent();
            };

            this.AgregarTabNubetico(tabNubetico);
        }

        private void OnAbrirClick(MouseEventArgs args)
        {
            if (UsuariosSeleccionados.Count == 0)
                return;

            var usuarioSeleccionado = UsuariosSeleccionados.FirstOrDefault();

            if (usuarioSeleccionado != null)
                AbrirDetalleUsuario(usuarioSeleccionado, TipoEstadoControl.Lectura);
        }

        private void OnEditarClick(MouseEventArgs args)
        {
            if (UsuariosSeleccionados.Count == 0)
                return;

            var usuarioSeleccionado = UsuariosSeleccionados.FirstOrDefault();

            if (usuarioSeleccionado != null)
                AbrirDetalleUsuario(usuarioSeleccionado, TipoEstadoControl.Edicion);
        }

        private async Task OnRefrescarClickAsync(MouseEventArgs args)
        {
            await RefreshGridAsync("NombreCompleto asc", this.RowsPerPage, 0);
        }

        private void OnCerrarClick(MouseEventArgs args)
        {
            this.CerrarTabNubetico();
        }

        private void AbrirDetalleUsuario(UsuarioNubeticoGridDto usuarioSeleccionado, TipoEstadoControl estadoControl)
        {
            string? guidUsuario = usuarioSeleccionado.UUID.ToString();

            // Crear instancia TabNubetico
            TabNubetico tabNubetico = new TabNubetico
            {
                EstadoControl = estadoControl,
                Icono = estadoControl == TipoEstadoControl.Edicion
                            ? MenuItemsFactory.MenuIconDictionary["editar"]
                            : this.IconoBase,

                Text = $"{Localizer["Core.Users.User"]} [{usuarioSeleccionado.Username}]",
                TipoControl = typeof(UsuariosDetComponent),
                Repetir = true
            };

            // Instanciar componente contenido en TabNubetico
            tabNubetico.Componente = builder =>
            {
                builder.OpenComponent(0, tabNubetico.TipoControl);
                builder.AddAttribute(1, "GuidUsuario", guidUsuario);
                builder.AddComponentReferenceCapture(1, instance =>
                {
                    // Asegurarnos que el componente interno instanciado hereda el componente base
                    if (instance is NbBaseComponent nbComponent)
                    {
                        tabNubetico.InstanciaComponente = nbComponent;
                        // Establecer el estado inicial para el componente
                        nbComponent.EstadoControl = estadoControl;

                        if (!string.IsNullOrEmpty(this.IconoBase))
                            nbComponent.IconoBase = this.IconoBase;

                        nbComponent.TriggerMenuUpdate();
                    }
                });
                builder.CloseComponent();
            };

            this.AgregarTabNubetico(tabNubetico);
        }

        private async Task DataGridRowDoubleClick(DataGridRowMouseEventArgs<UsuarioNubeticoGridDto> args)
        {
            if (args.Data != null)
                AbrirDetalleUsuario(args.Data, TipoEstadoControl.Lectura);
        }

        private async Task LoadDataAsync(LoadDataArgs args)
        {
            string orderBy = string.Join(",", args.Sorts.Select(s => $"{s.Property} {(s.SortOrder == SortOrder.Descending ? "desc" : "asc")}"));
            await RefreshGridAsync(orderBy, args.Top ?? 0, args.Skip ?? 0);
        }

        private async Task RefreshGridAsync(string orderBy, int top, int skip)
        {
            IsLoading = true;

            var result = await UsuariosService.GetUsuariosPaginado(top, skip, orderBy, Filtro.Username, Filtro.Nombre, Filtro.IdEstadoUsuario);

            if (!result.Success || result.Data == null)
            {
                IsLoading = false;
                ListaUsuarios.Clear();
                Count = 0;
                return;
            }

            PaginatedListDto<UsuarioNubeticoGridDto>? listaPaginada = JsonConvert.DeserializeObject<PaginatedListDto<UsuarioNubeticoGridDto>>(result.Data.ToString());

            if (listaPaginada != null)
            {
                ListaUsuarios = listaPaginada.Data;
                Count = listaPaginada.RecordsTotal;
            }

            IsLoading = false;
        }
    }

    public class FiltroUsuariosNubeticoGridDto
    {
        public string Username { get; set; } = "";
        public string Nombre { get; set; } = "";
        public int? IdEstadoUsuario { get; set; }
    }

}
