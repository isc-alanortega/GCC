using Microsoft.AspNetCore.Components;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Frontend.Services.Core;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Dto.ProyectosConstruccion.Contratistas;

namespace Nubetico.Frontend.Components.Core.Shared
{
    public partial class EntidadContratistaComponent : NbBaseComponent
    {
        #region Propiedades
        [Inject] protected EntidadesService entidadesService { get; set; }
        [Inject] protected SuppliesService insumosService { get; set; }
        [Inject] protected FoliadorService foliosService { get; set; }
        [Parameter] public ContratistasDto ContratistaData { get; set; }
        [Parameter] public TipoEstadoControl EstadoControl { get; set; }
        private List<TablaRelacionDto> LstRegimenesFiscales = new();
        private List<TablaRelacionDto> LstTipoRegimenesFiscales = new();
        private List<TablaRelacionDto> LstTipoFormaPago = new();
        private List<TablaRelacionStringDto> LstUsoCFDI = new();
        private List<TablaRelacionStringDto> LstTipoMetodoPago = new();
        private bool IsCreditEnabled => ContratistaData.Credito;
        private Dictionary<string, List<string>> FormValidationErrors { get; set; } = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            LstRegimenesFiscales = await entidadesService.GetAllTipoRegimenFiscal();
            LstTipoRegimenesFiscales = await entidadesService.GetAllRegimenFiscal();
            LstTipoFormaPago = await entidadesService.GetAllFormaPago();
            LstTipoMetodoPago = await entidadesService.GetAllMetodoDePago();
            LstUsoCFDI = await entidadesService.GetAllUsoCFDI();
        }

        #region Funciones


        public bool ValidateContratista()
        {
            FormValidationErrors.Clear();

            void AddError(string field, string msg)
            {
                if (!FormValidationErrors.ContainsKey(field))
                    FormValidationErrors[field] = new List<string>();
                FormValidationErrors[field].Add(msg);
            }

            // RFC
            if (string.IsNullOrWhiteSpace(ContratistaData.Rfc))
                AddError("Rfc", "RFC es obligatorio.");

            // Nombre Comercial
            if (string.IsNullOrWhiteSpace(ContratistaData.NombreComercial))
                AddError("NombreComercial", "Nombre Comercial es obligatorio.");

            // Razón Social
            if (string.IsNullOrWhiteSpace(ContratistaData.RazonSocial))
                AddError("RazonSocial", "Razón Social es obligatorio.");

            // Tipo persona SAT
            if (ContratistaData.IdTipoPersonaSat == 0)
                AddError("IdTipoPersonaSat", "Debes elegir persona física o moral.");

            // Tipo régimen fiscal
            if (ContratistaData.IdTipoRegimenFiscal == 0)
                AddError("IdTipoRegimenFiscal", "Debes seleccionar un tipo de régimen.");

            // Régimen fiscal
            if (ContratistaData.IdRegimenFiscal == 0)
                AddError("IdRegimenFiscal", "Debes seleccionar un régimen fiscal.");

            // Método de pago
            if (ContratistaData.IdTipoMetodoPago == 0)
                AddError("IdTipoMetodoPago", "Debes elegir un método de pago.");

            // Uso CFDI
            if (ContratistaData.IdUsoCFDI == 0)
                AddError("IdUsoCFDI", "Debes elegir un uso de CFDI.");

            // Forma de pago
            if (ContratistaData.IdFormaPago == 0)
                AddError("IdFormaPago", "Debes elegir la forma de pago.");

            // Email
            if (string.IsNullOrWhiteSpace(ContratistaData.Email))
                AddError("Email", "Email es obligatorio.");

            // Teléfono
            if (string.IsNullOrWhiteSpace(ContratistaData.Telefono))
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