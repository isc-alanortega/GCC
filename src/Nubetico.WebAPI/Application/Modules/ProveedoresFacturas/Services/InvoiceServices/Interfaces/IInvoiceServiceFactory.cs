namespace Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services.InvoiceServices.Interfaces
{
    public interface IInvoiceServiceFactory
    {
        public (IValidateXml? validator, ISendInvoice? sender) GetServicesForTenant(string tenant);
    }
}
