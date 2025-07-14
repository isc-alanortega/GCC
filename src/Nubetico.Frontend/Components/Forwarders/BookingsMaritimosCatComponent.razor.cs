using Nubetico.Shared.Dto.PortalProveedores;
using Radzen.Blazor;
using Radzen;

namespace Nubetico.Frontend.Components.Forwarders
{
    public partial class BookingsMaritimosCatComponent
    {
        private RadzenDataGrid<ObraDto>? grid;
        private IEnumerable<ObraDto> ListObraDto;
        private int Count;
        private bool IsLoading = false;

        private async Task LoadData(LoadDataArgs args)
        {
            IsLoading = true;
            IsLoading = false;
        }
    }
}
