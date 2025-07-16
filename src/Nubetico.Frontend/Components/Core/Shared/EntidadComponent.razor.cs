using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Nubetico.Frontend.Services.Core;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.Core.Folios;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;

namespace Nubetico.Frontend.Components.Core.Shared
{
	public partial class EntidadComponent
	{
		#region Propiedades
		[Inject] protected EntidadesService entidadesService {  get; set; }
        [Inject] protected SuppliesService insumosService { get; set; }
        [Inject] protected FoliadorService foliosService { get; set; }
		[Parameter]  public ProveedorGetDto? ProveedorData { get; set; }
        private List<TablaRelacionDto> LstRegimenesFiscales = new();
        private List<TablaRelacionDto> LstTipoRegimenesFiscales = new();
        private List<TablaRelacionDto> LstTipoFormaPago = new();
        private List<TablaRelacionDto> LstTipoInsumo = new();
        private List<TablaRelacionStringDto> LstUsoCFDI = new();
        private List<TablaRelacionStringDto> LstTipoMetodoPago = new();
        private List<TablaRelacionDto> LstTipo = new();

        #endregion
        protected override async Task OnInitializedAsync()
        {
            LstRegimenesFiscales = await entidadesService.GetAllTipoRegimenFiscal();
            LstTipoRegimenesFiscales = await entidadesService.GetAllRegimenFiscal();
            LstTipoFormaPago = await entidadesService.GetAllFormaPago();
            LstTipoMetodoPago = await entidadesService.GetAllMetodoDePago();
            LstTipoInsumo = await insumosService.GetAllTipoInsumo();
            LstUsoCFDI = await entidadesService.GetAllUsoCFDI();
            LstTipo = await insumosService.GetAllInsumos();
            //var request = new FolioRequestDto
            //{
            //    Alias = "core.entidades.proveedores",
            //    IdSucursal = null 
            //};

            //var folioResult = await foliosService.PostGetFolioAsync(request);
            //string folioSolicitud = $"{folioResult.Serie}{folioResult.Folio.ToString($"D{folioResult.Digitos}")}";
            //ProveedorData.Folio = folioSolicitud;
        }
        #region Funciones


        private bool GetDisabled(string? field_name = null)
		{
			//return EstadoControl == TipoEstadoControl.Lectura;
			return false;
		}

		#endregion
	}
}
