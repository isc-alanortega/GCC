namespace Nubetico.DAL.ResultSets.Core
{
    public class FolioResultSet
    {
        /// <summary>
        /// Prefijo del folio, puede ser nulo
        /// </summary>
        public string Serie { get; set; } = string.Empty;
        /// <summary>
        /// Número de dígitos, puede ser nulo
        /// </summary>
        public int Digitos { get; set; }
        /// <summary>
        /// Número de folio a utilizar
        /// </summary>
        public int Folio { get; set; }
    }
}
