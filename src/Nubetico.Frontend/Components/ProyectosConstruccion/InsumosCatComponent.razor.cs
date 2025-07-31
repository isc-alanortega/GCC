using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Supplies;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class InsumosCatComponent : NbBaseComponent
    {
        #region INJECTIONS
        [Inject] private IStringLocalizer<SharedResources>? Localizer { get; set; }
        [Inject] private NotificationService? NotificationService { get; set; }
        [Inject] SuppliesService SuppliesDA { get; set; }
        #endregion

        public List<GroupsCatalogDto> TypesSupplies { get; set; } = [];
        RadzenDataGrid<InsumosDto>? Grid;
        private IEnumerable<InsumosDto>? SuppliesList { get; set; }
        private IList<InsumosDto> SelectedSupply { get; set; } = [];
        private SuppliesPaginatedRequestDto RequestForm { get; set; } = new();

        private int Count { get; set; }
        private bool IsLoading { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await GetFetcher();
            await base.OnInitializedAsync();
        }

        public async Task LoadData(LoadDataArgs args)
        {
            if (IsLoading) return;

            try
            {
                // Procesar ordenamiento
                // Configurar paginación y ordenamiento
                var limit = args.Top ?? 20;
                var offset = args.Skip ?? 0;
                var orderBy = args.Sorts != null && args.Sorts.Any()
                    ? string.Join(",", args.Sorts.Select(s => $"{s.Property} {(s.SortOrder == SortOrder.Descending ? "desc" : "asc")}"))
                    : "Code asc";


                RequestForm.Limit = limit;
                RequestForm.Offset = offset;
                // Obtener datos paginados
                var result = await SuppliesDA.GetPaginatedSupplies(request: RequestForm);

                if (result != null && result.Success && result.Data != null)
                {
                    SuppliesList = result.Data!.Data;
                    Count = result.Data!.RecordsTotal;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al cargar datos: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        private async Task GetFetcher()
        {
            try
            {
                var response = await SuppliesDA.GetFetcherForm();
                if (response == null || !response.Success || response.Data == null || response.StatusCode > 300) return;

                TypesSupplies = response!.Data!.TypesSupplies.ToList();
            }
            catch (Exception ex)
            {
                NotifyAcces(summary: Localizer!["Sharde.Text.ProblemOcurred"], details: Localizer!["Shared.Text.UnknowError"], severity: NotificationSeverity.Error);
            }
        }

        private void NotifyAcces(string summary, string details, NotificationSeverity severity) => NotificationService!.Notify(new()
        {
            Severity = severity,
            Summary = summary,
            Detail = details
        });

        private BadgeStyle GetBadgeStyle(int id_type) => id_type switch
        {
            2 => BadgeStyle.Info,
            3 => BadgeStyle.Warning,
            _ => BadgeStyle.Base,
        };

        private async void DataGridRowDoubleClick(DataGridRowMouseEventArgs<InsumosDto> supply)
        {
            if (supply.Data == null)
            {
                NotifyAcces("", "", NotificationSeverity.Error);
                return;
            }

            await HandleOpenAsync(supply.Data.ID, TipoEstadoControl.Lectura);
        }

        private async Task HandleOpenAsync(int supplyId, TipoEstadoControl action)
        {
            var supplies = await GetSuppliesById(supplyId);
            if (supplies == null) return;

            OpenTab(action: action, name: $"{Localizer!["Shared.Text.Supply"]} {supplies.Description}", data: supplies);
        }

        private async Task<SuppliesDto?> GetSuppliesById(int suppliesId)
        {
            Console.WriteLine("GetSuppliesById");
            var result = await SuppliesDA.GetSuppliesById(suppliesId);
            if (result == null || !result.Success)
            {
                NotifyAcces("", "", NotificationSeverity.Error);
                return null;
            }

            return result!.Data;
        }

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
                    menu!.Click = onClick;
                    menusMostrar.Add(menu);
                }
            }

            AgregarMenuSiExiste(BaseMenuCommands.OPEN, EventCallback.Factory.Create<MenuItemEventArgs>(this, () => OnClickOpenAsync(TipoEstadoControl.Lectura)));
            AgregarMenuSiExiste(BaseMenuCommands.ADD, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickAdd));
            AgregarMenuSiExiste(BaseMenuCommands.EDIT, EventCallback.Factory.Create<MenuItemEventArgs>(this, () => OnClickOpenAsync(TipoEstadoControl.Edicion)));
            AgregarMenuSiExiste(BaseMenuCommands.REFRESH, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickLoadData));
            AgregarMenuSiExiste(BaseMenuCommands.CLOSE, EventCallback.Factory.Create<MenuItemEventArgs>(this, OnClickClose));

            return menusMostrar;
        }

        private async void OnClickOpenAsync(TipoEstadoControl action)
        {
            var supply = SelectedSupply.FirstOrDefault();
            Console.Write(supply);

            if (supply == null)
            {
                NotifyAcces("OCURRIO UN ERRORRR", "", NotificationSeverity.Error);
                return;
            }

            await HandleOpenAsync(supply.ID, action);
        }

        private void OnClickAdd() => OpenTab(action: TipoEstadoControl.Alta, name: $"{Localizer!["Shared.Textos.New"]} {Localizer["Shared.Text.Supply"]}", data: new SuppliesDto());

        private async void OnClickLoadData()
        {
            var dataArg = new LoadDataArgs();
            await LoadData(dataArg);
        }

        private void OnClickClose() => this.CerrarTabNubetico();

        public void OpenTab(TipoEstadoControl action, string name, SuppliesDto data)
        {
            // Crear instancia TabNubetico
            TabNubetico tabNubetico = new()
            {
                EstadoControl = action,
                Icono = GetIconByControlState(action),
                Text = name,
                TipoControl = typeof(SuppliesDetComponent),
                Repetir = true
            };

            // Instanciar componente contenido en TabNubetico
            tabNubetico.Componente = builder =>
            {
                builder.OpenComponent(0, tabNubetico.TipoControl);
                builder.AddAttribute(1, "SupplyData", data);
                builder.AddComponentReferenceCapture(1, instance =>
                {
                    // Asegurarnos que el componente interno instanciado hereda el componente base
                    if (instance is NbBaseComponent nbComponent)
                    {
                        tabNubetico.InstanciaComponente = nbComponent;
                        // Establecer el menú inicial para el componente
                        nbComponent.IconoBase = GetIconByControlState(TipoEstadoControl.Lectura);
                        nbComponent.EstadoControl = action;
                        nbComponent.TriggerMenuUpdate();
                    }
                });
                builder.CloseComponent();
            };

            this.AgregarTabNubetico(tabNubetico);
        }

        private static string GetIconByControlState(TipoEstadoControl typeState) => typeState switch
        {
            TipoEstadoControl.Edicion => $"{MenuItemsFactory.MenuIconDictionary["editar"]}",
            TipoEstadoControl.Alta => $"{MenuItemsFactory.MenuIconDictionary["agregar"]}",
            _ => "e58d" // fa-truck-field
        };
    }
}
