using System;
using System.Collections.Generic;
using System.Linq;
namespace Nubetico.Shared.Dto.PortalClientes
{
    public class ExternalClientInvoices
    {
        // <summary>
        /// Invoice folio with serial and folio
        /// </summary>
        public string Folio { get; set; }
        // <summary>
        /// Invoicetype
        /// </summary>
        public string InvoiceType { get; set; }
        // <summary>
        /// Invoice creation date
        /// </summary>
        public DateTime Date {  get; set; }
        // <summary>
        /// Invoice business name
        /// </summary>
        public string BusinessName { get; set; }
        // <summary>
        /// Invoice total
        /// </summary>
        public decimal Total {  get; set; }
        // <summary>
        /// Invoice balance
        /// </summary>
        public decimal? Balance { get; set; }
        // <summary>
        /// Invoice status
        /// </summary>
        public string Status { get; set; }
        // <summary>
        /// Invoice indicator to mark as paid
        /// </summary>
        public bool Payed { get; set; }
    }
}
