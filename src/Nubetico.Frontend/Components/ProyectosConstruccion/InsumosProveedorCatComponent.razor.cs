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
    public partial class InsumosProveedorCatComponent
    {

        private RadzenDataGrid<InsumosDto>? GridInsumos;
        private bool IsLoading { get; set; } = false;
        private int Count { get; set; }
        private IEnumerable<InsumosDto>? SuppliesList { get; set; }
        private SuppliesPaginatedRequestDto RequestForm { get; set; } = new();


        [Inject] SuppliesService SuppliesDA { get; set; }






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
                    //SuppliesList = result.Data!.Data;
                    var insumos = result.Data!.Data.Where(i => i.Type != "MANO DE OBRA").ToList();
                    SuppliesList = insumos;
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
                //StateHasChanged();
            }
        }

        private BadgeStyle GetBadgeStyle(int id_type) => id_type switch
        {
            2 => BadgeStyle.Info,
            3 => BadgeStyle.Warning,
            _ => BadgeStyle.Base,
        };
    }
}
