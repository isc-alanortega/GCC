namespace Nubetico.WebAPI.Application.External.CIEmail.Dto
{
    public class CIEmailRequestDto
    {
        public string CorreoOrigen { get; set; }
        public int DominioOrigen { get; set; }
        public string CorreoDestinatario { get; set; }
        public string Asunto { get; set; }
        public string MensajeTXT { get; set; }
        public string MensajeHTML { get; set; }
        public List<string> NombresAdjuntos { get; set; } = new List<string>();

        /// <summary>
        /// Archivos codificados en Base64, deben ser la misma cantidad de cadenas y en el mismo orden que en la propiedad NombresAdjuntos
        /// </summary>
        public List<string> ContenidosAdjuntos { get; set; } = new List<string>();
    }
}
