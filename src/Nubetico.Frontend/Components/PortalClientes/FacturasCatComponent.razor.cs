using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Nubetico.Frontend.Components.Dialogs;
using Nubetico.Frontend.Services.ProveedoresFacturas;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProveedoresFacturas;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend.Components.PortalClientes
{
    public partial class FacturasCatComponent
    {
        [Inject]
        private FacturasService _facturasService { get; set; }
        private Entidad_Simplificado? ProviderData { get; set; }

        #region Variables

        //Variables a usar en los filtros
        DateTime? fechaInicial = DateTime.Now;
        DateTime? fechaFinal = DateTime.Now;
        private List<string> ListaEstadosFacturas = new List<string> { "Todas", "Subida", "Autorizada", "Rechazada", "Pagada" };
        private string estadoSeleccionado = "Todas";

        //Variables del grid
        private RadzenDataGrid<FacturaDto>? GridFacturasProveedores { get; set; }
        private List<FacturaDto>? ListaFacturas { get; set; }
        private int Count { get; set; }
        private bool IsLoading { get; set; } = false;
        public IList<FacturaDto> FacturaSeleccionada { get; set; } = new List<FacturaDto>();
        #endregion



        #region Funciones

        private async Task LoadData(LoadDataArgs args)
        {
            IsLoading = true;

            var usuario = await GetUsuarioAutenticadoAsync();
            string? usuarioCorreo = usuario.FindFirst("email")?.Value;
            var proveedorData = await _facturasService.GetProveedorAsync(usuarioCorreo);

            if (proveedorData != null && proveedorData.Success && proveedorData.Data != null)
            {
                ProviderData = proveedorData.Data;
            }
            else
            {
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Info, Summary = "Consulta proveedor nula", Detail = "La repuesta al parecer regresó un valor nulo." });
            }

            IsLoading = false;
        }

        private async Task ActualizarGrid(string estatus)
        {
            IsLoading = true;

            ApiFacturaPeticion peticion = new ApiFacturaPeticion();
            peticion.ID_Entidad = ProviderData.ID_Entidad;
            peticion.Fecha_Inicio = fechaInicial.Value;
            peticion.Fecha_Fin = fechaFinal.Value;
            peticion.Estatus = estatus switch
            {
                "Subida" => 1,
                "Autorizada" => 2,
                "Rechazada" => 3,
                "Pagada" => -1,
                _ => 0
            };

            var facturasData = await _facturasService.GetFacturasAsync(peticion);

            if (facturasData != null && facturasData.Success)
            {
                var _ListaFacturas = JsonConvert.DeserializeObject<PaginatedListDto<FacturaDto>>(facturasData.Data.ToString());
                ListaFacturas = _ListaFacturas.Data;
                Count = ListaFacturas.Count;
            }
            else
            {
                NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Info, Summary = "Consulta proveedor nula", Detail = "La repuesta al parecer regresó un valor nulo." });
            }

            IsLoading = false;
        }

        private BadgeStyle GetBadgeColor(string estatus) => estatus switch
        {
            "Rechazada" => BadgeStyle.Danger,
            "Pagada" => BadgeStyle.Success,
            _ => BadgeStyle.Light,
        };


        private void CargarCFDI(string text)
        {
            NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Info, Summary = "Llamar función para grabar un CFDI", Detail = text });
        }

        private async Task OpenUploadXmModal()
        {
            var result = await DialogService.OpenAsync<UploadXmlDialogComponet>(title: "Cargar nuevo CFDI",
            parameters: new Dictionary<string, object>() { { "ProviderData", ProviderData } },
            options: new DialogOptions()
            {
                CloseDialogOnOverlayClick = true,
                AutoFocusFirstElement = true,
                CloseDialogOnEsc = true,
                Width = "700px"
            });

            if (result != null && result)
            {
                ActualizarGrid(estadoSeleccionado);
                StateHasChanged();
            }
        }

        #endregion
    }

    public class EstatusFactura
    {
        public int ID { get; set; }
        public string RazonSocial { get; set; }
        public string Serie { get; set; }
        public string Folio { get; set; }
        public string Moneda { get; set; }
        public DateTime FechaFactura { get; set; }
        public string Estatus { get; set; }

        public int IDMovimiento { get; set; }
        public int? IDCargoAbono { get; set; }
        public int? Secuencia { get; set; }
        public int? IDProveedor { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string IDCompra { get; set; }
        public short? IDTipoCargoAbono { get; set; }
        public string Tipo { get; set; }
        public short? IDFormaPago { get; set; }
        public string Referencia { get; set; }
        public string Numero { get; set; }
        public string Banco { get; set; }
        public float? Monto { get; set; }
        public float? Importe { get; set; }
        public float? Total { get; set; }
        public float? Restante { get; set; }
        public bool Pagado { get; set; }
        public string Observaciones { get; set; }
        public int? IDMoneda { get; set; }
        public float? Conversion { get; set; }
        public float? SubTotal { get; set; }
        public float? Impuesto1 { get; set; }
        public float? Impuesto2 { get; set; }
        public float? Retencion1 { get; set; }
        public float? Retencion2 { get; set; }
        public float? GranTotal { get; set; }
        public int? IDCosto { get; set; }
        public short? Estado { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public bool? CFDI { get; set; }
        public string UUID { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int? IDUsuarioAlta { get; set; }
        public int? IDUsuarioCancelacion { get; set; }
        public int? EstatusProceso { get; set; }
    }
}
