using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Frontend.Services.Core;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.Core.Folios;
using Nubetico.Shared.Dto.ProyectosConstruccion;
using Nubetico.Shared.Dto.ProyectosConstruccion.Proveedores;

namespace Nubetico.Frontend.Components.Core.Shared
{
	public partial class EntidadComponent :NbBaseComponent
	{
		#region Propiedades
		[Inject] protected EntidadesService entidadesService {  get; set; }
        [Inject] protected SuppliesService insumosService { get; set; }
        [Inject] protected FoliadorService foliosService { get; set; }
		[Parameter]  public ProveedorGetDto? ProveedorData { get; set; }
        [Parameter] public TipoEstadoControl EstadoControl { get; set; }
        private List<TablaRelacionDto> LstRegimenesFiscales = new();
        private List<TablaRelacionDto> LstTipoRegimenesFiscales = new();
        private List<TablaRelacionDto> LstTipoFormaPago = new();
        private List<TablaRelacionDto> LstTipoInsumo = new();
        private List<TablaRelacionStringDto> LstUsoCFDI = new();
        private List<TablaRelacionStringDto> LstTipoMetodoPago = new();
        private List<TablaRelacionDto> LstTipo = new();
        private bool IsCreditEnabled => ProveedorData.Credito;
        // Guardo los errores de validación por campo
        private Dictionary<string, List<string>> FormValidationErrors { get; set; }
            = new Dictionary<string, List<string>>();
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

        public bool ValidateProveedor()
        {
            FormValidationErrors.Clear();

            void AddError(string field, string msg)
            {
                if (!FormValidationErrors.ContainsKey(field))
                    FormValidationErrors[field] = new List<string>();
                FormValidationErrors[field].Add(msg);
            }

            // RFC
            if (string.IsNullOrWhiteSpace(ProveedorData.Rfc))
                AddError("RFC", "RFC es obligatorio.");
            // Nombre Comercial
            if (string.IsNullOrWhiteSpace(ProveedorData.NombreComercial))
                AddError("NombreComercial", "Nombre Comercial es obligatorio.");
            //Razon Social
            if (string.IsNullOrWhiteSpace(ProveedorData.NombreComercial))
                AddError("RazónSocial", "Razón Social es obligatorio.");

            // Tipo persona SAT
            if (ProveedorData.IdTipoPersonaSat == null || ProveedorData.IdTipoPersonaSat == 0)
                AddError("IdTipoPersonaSAT", "Debes elegir persona física o moral.");

            // Tipo proveedor
            if (ProveedorData.IdTipoProveedor == null || ProveedorData.IdTipoProveedor == 0)
                AddError("IdTipoProveedor", "Debes seleccionar un tipo.");

            // Tipo régimen fiscal
            if (ProveedorData.IdTipoRegimenFiscal == null || ProveedorData.IdTipoRegimenFiscal == 0)
                AddError("IdTipoRegimenFiscal", "Debes seleccionar un tipo de régimen.");

            // Régimen fiscal
            if (ProveedorData.IdRegimenFiscal == null ||  ProveedorData.IdRegimenFiscal == 0)
                AddError("IdRegimenFiscal", "Debes seleccionar un régimen fiscal.");

            // Método de pago
            if (ProveedorData.IdTipoMetodoPago == null ||ProveedorData.IdTipoMetodoPago == 0)
                AddError("IdTipoMetodoPago", "Debes elegir un método de pago.");

            // Uso CFDI
            if (ProveedorData.IdUsoCFDI == null || ProveedorData.IdUsoCFDI == 0)
                AddError("IdUsoCFDI", "Debes elegir un uso de CFDI.");
            
            // Forma Pago
            if (ProveedorData.IdFormaPago == null || ProveedorData.IdFormaPago == 0)
                AddError("IdFormaPago", "Debes elegir la forma de pago.");
            
            //Email
            if (ProveedorData.Email == null || string.IsNullOrWhiteSpace(ProveedorData.Email))
                AddError("Email", "Email es obligatorio.");
            
            //Telefono
            if (ProveedorData.Telefono == null || string.IsNullOrWhiteSpace(ProveedorData.Telefono))
                AddError("Telefono", "Teléfono es obligatorio.");

            return FormValidationErrors.Count == 0;
        }

        private bool GetDisabled(string? field_name = null)
		{
			return EstadoControl == TipoEstadoControl.Lectura;
		}

		#endregion
	}
}
