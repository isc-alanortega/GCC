using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Nubetico.Frontend.Services.PortalProveedores;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.PortalProveedores;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.PortalProveedores
{
    public partial class ObrasCatComponent
    {
        [Inject]
        private ObraService _obrasService { get; set; }

        RadzenDataGrid<ObraDto>? grid;
        private IEnumerable<ObraDto> ListObraDto;
        private int Count;
        private bool IsLoading = false;

        private async Task Reset()
        {
            if (grid != null)
            {
                grid.Reset(true);
                await grid.FirstPage(true);
            }
        }

        private async Task LoadData(LoadDataArgs args)
        {
            IsLoading = true;

            string orderBy = string.Join(",", args.Sorts.Select(s => $"{s.Property} {(s.SortOrder == SortOrder.Descending ? "desc" : "asc")}"));

            var result = await _obrasService.GetPaginadoAsync(args.Skip ?? 0, args.Top ?? 0, orderBy);

            if (!result.Success || result.Data == null)
            {
                return;
            }

            PaginatedListDto<ObraDto>? listaPaginada = JsonConvert.DeserializeObject<PaginatedListDto<ObraDto>>(result.Data.ToString());

            if (listaPaginada != null)
            {
                ListObraDto = listaPaginada.Data;
                Count = listaPaginada.RecordsTotal;
            }

            IsLoading = false;
        }
    }
}
