using Microsoft.AspNetCore.Components;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Shared.Dto.PortalClientes;

namespace Nubetico.Frontend.Components.Dialogs.PortalClientes
{

    public partial class ShowInvoiceDialogComponent : NbBaseComponent
    {
		[Parameter]
		public ExternalClientInvoices Invoice { get; set; }
        [Parameter]
        public string InvoiceUrl { get; set; }
    }
}
