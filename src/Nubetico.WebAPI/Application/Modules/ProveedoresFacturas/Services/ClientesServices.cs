using Nubetico.Shared.Dto.ProveedoresFacturas;
using Nubetico.WebAPI.Application.External.ProveedoresFacturas;

namespace Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services
{
    public class ClientesServices
    {
        public async Task<Entidad_Simplificado> GetProvedorxCorreo(string correo)
        {
            ApiRespuesta<Entidad_Simplificado> respuesta = await ClientesEndpoints.GetProvedorxCorreo(correo);
            return (respuesta.resultado);
        }


        //public async Task<bool> SendInvoiceProvider(object invoice)
        //{
        //    var respuesta = await ClientesEndpoints.SendInvoiceProivider(invoice);
        //    return (respuesta.resultado);
        //}
    }
}
