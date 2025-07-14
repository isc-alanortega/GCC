namespace Nubetico.Shared.Dto.Core
{
    public class XmlElementsDto
    {
        #region cfdi:Comprobante
        public string? CondicionesDePago { get; set; } = null;
        public string? Exportacion { get; set; } = null;
        public string? Fecha { get; set; } = null;
        public string? Folio { get; set; } = null;
        public string? FormaPago { get; set; } = null;
        public string? LugarExpedicion { get; set; } = null;
        public string? MetodoPago { get; set; } = null;
        public string? Moneda { get; set; } = null;
        public string? NoCertificado { get; set; } = null;
        public string? SubTotal { get; set; } = null;
        public string? TipoCambio { get; set; } = null;
        public string? TipoDeComprobante { get; set; } = null;
        public string? Total { get; set; } = null;
        public string? Serie { get; set; } = null;
        #endregion


        #region cfdi:Emisor
        public string? Emisor { get; set; } = null;
        public string? RegimenFiscalEmisor { get; set; } = null;
        public string? RfcEmisor { get; set; } = null;
        #endregion


        #region cfdi:Receptor
        public string? Receptor { get; set; } = null;
        public string? DomicilioFiscalReceptor { get; set; } = null;
        public string? NombreReceptor { get; set; } = null;
        public string? RegimenFiscalReceptor { get; set; } = null;
        public string? RfcReceptor { get; set; } = null;
        public string? UsoCFDI { get; set; } = null;
        #endregion


        #region cfdi:Impuestos>
        public string? Traslado { get; set; } = null;
        public string? Retencion { get; set; } = null;
        #endregion


        #region <cfdi:Complemento> <tfd:TimbreFiscalDigital
        public string? UUID { get; set; } = null;
        #endregion


        #region Elements 
        public string? Banco { get; set; } = null;

        #endregion
    }
}
