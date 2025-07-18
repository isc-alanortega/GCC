﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Nubetico.Frontend.Components.Core.Shared;
using Nubetico.Frontend.Models.Enums.Core;
using Nubetico.Frontend.Models.Static.Core;
using Nubetico.Frontend.Services.Core;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Radzen;
using Radzen.Blazor;
using System.Globalization;



using Newtonsoft.Json;
using Nubetico.Frontend.Models.Class.Core;
using Nubetico.Frontend.Models.Enums.Core;
using Microsoft.AspNetCore.Components;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Supplies;

namespace Nubetico.Frontend.Components.ProyectosConstruccion
{
    public partial class ContratistaDetComponent : NbBaseComponent
    {
        [Parameter]
        public string? GuidContratista { get; set; }
        [Parameter]
        public ContratistasDto ContratistaData { get; set; }

        [Parameter]
        public TipoEstadoControl EstadoControl { get; set; }

		//[Parameter]
		//public ContratistasDto ContratistaData = new ContratistasDto();

		// ***************************************************************************************************
		// Solo para insumos
		// ***************************************************************************************************
		RadzenDataGrid<InsumosDto>? GridInsumos;
		private bool IsLoading { get; set; } = false;
		private int Count { get; set; }
		private IEnumerable<InsumosDto>? SuppliesList { get; set; }
		[Inject] SuppliesService SuppliesDA { get; set; }
		private SuppliesPaginatedRequestDto RequestForm { get; set; } = new();

		// ***************************************************************************************************

		public bool tieneCredito = true;

        private LotsDetail LotData { get; set; } = new LotsDetail();
        private DomicilioComponent AddressComponent { get; set; }
        private IDictionary<string, string> IconTabs { get; set; } = new Dictionary<string, string>();

        protected override void OnInitialized()
        {
            breakpointService!.OnChange += StateHasChanged;
            base.OnInitialized();
        }

        public void Dispose()
        {
            breakpointService!.OnChange -= StateHasChanged;
        }



        private bool GetDisabled(string? field_name = null)
        {
            return EstadoControl == TipoEstadoControl.Lectura;
        }

        public int GetColumnsSize(string? field_name = null)
        {
            var breakpoint = breakpointService.GetCurrentBreakpoint();
            if (field_name == "BETWEENSTREET")
            {
                switch (breakpoint)
                {
                    case Breakpoint.Xs:
                        return 12;
                    case Breakpoint.Sm:
                    case Breakpoint.Md:
                        return 6;
                    default:
                        return 5;
                }
            }
            else
            {
                switch (breakpoint)
                {
                    case Breakpoint.Xs:
                        return 12;
                    case Breakpoint.Sm:
                    case Breakpoint.Md:
                        return 6;
                    default:
                        return 4;
                }
            }
        }

        private string GetCustomValue(string field_name)
        {
            switch (field_name)
            {
                case "SURFACEMEASURE":
                    return LotData.SurfaceMeasure.HasValue ? $"{LotData.SurfaceMeasure?.ToString("0.###")} m2" : "0 m2";
                case "GENERALTAB":
                    bool icon = IconTabs.TryGetValue(field_name, out string? generalkvp);
                    return string.IsNullOrEmpty(generalkvp) ? "" : generalkvp;
                case "LOCATIONTAB":
                    icon = IconTabs.TryGetValue(field_name, out string? locationkvp);
                    return string.IsNullOrEmpty(locationkvp) ? "" : locationkvp;
                default:
                    return "";
            }
        }

		// ***********************************************************************************************
		// Solo para insumos
		// ***********************************************************************************************

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
					var insumos = result.Data!.Data.Where(i => i.Type == "MANO DE OBRA").ToList();
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
				StateHasChanged();
			}
		}

		private BadgeStyle GetBadgeStyle(int id_type) => id_type switch
		{
			2 => BadgeStyle.Info,
			3 => BadgeStyle.Warning,
			_ => BadgeStyle.Base,
		};

		// ***********************************************************************************************
	}
}
