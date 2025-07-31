using Nubetico.Frontend.Models.Class.Core;
using Radzen.Blazor;
using static Nubetico.Frontend.Components.Shared.NbBaseComponent;

namespace Nubetico.Frontend.Services.Core
{
    public class ComponentService
    {
        // Evento compartido para la interacción del panel con el home para agregar tabs
        public event Action<TabNubetico> OnMenuTabSelected;
        public void AddComponenteTabNubetico(TabNubetico tabNubetico)
        {
            OnMenuTabSelected?.Invoke(tabNubetico);
        }

        // Evento compartido de las acciones del MenuSuperiorComponent
        public event Action MenuChanged;
        private List<RadzenMenuItem> _currentMenuItems = new List<RadzenMenuItem>();
        public IReadOnlyList<RadzenMenuItem> CurrentMenuItems => _currentMenuItems;
        public void SetMenuItems(List<RadzenMenuItem> menuItems)
        {
            _currentMenuItems = menuItems;
            MenuChanged?.Invoke();
        }

        // Evento que se ejecuta al cambiar el nombre del Tab
        public event Action<string,string> TabNameChanged;
        public void SetCurrentTabName(string tabName, string iconText)
        {
            TabNameChanged?.Invoke(tabName,iconText);
        }

        public event Action<TipoEstadoControl> TabStateChanged;
        public void SetCurrentTabState(TipoEstadoControl tabState)
        {
            TabStateChanged?.Invoke(tabState);
        }

        // Evento que ocurre cuando el usuario cierra el tab desde el menu de opciones
        public event Action<bool> CloseCurrentTab;
        public void SetCloseCurrentTab(bool withWarning = false)
        {
            CloseCurrentTab?.Invoke(withWarning);
        }
    }
}
