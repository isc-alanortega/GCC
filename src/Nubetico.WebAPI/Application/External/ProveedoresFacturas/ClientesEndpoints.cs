using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProveedoresFacturas;
using RestSharp;

namespace Nubetico.WebAPI.Application.External.ProveedoresFacturas
{
    public class ClientesEndpoints
    {
        public static async Task<ApiRespuesta<Entidad_Simplificado>> GetProvedorxCorreo(string correo)
        {
            string rutaApi = "http://moregarwebapidemo.centralinformatica.com";
            string parametros = String.Format("/api/Clientes/GetEntidad?correo={0}", correo);
            var options = new RestClientOptions(rutaApi)
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest(parametros, Method.Get);
            request.AddHeader("Authorization", "Basic bTBSZWdhUl93NFBpOnZTcFIycFl6Z3hibitUUCtiTkVwUzc=");
            RestResponse response = await client.ExecuteAsync(request);

            ApiRespuesta<Entidad_Simplificado> respuesta = JsonConvert.DeserializeObject<ApiRespuesta<Entidad_Simplificado>>(response.Content!)!;
            return (respuesta);
        }

        public static async Task<ResponseDto<object>> SendInvoiceProvider(object invoiceData)
        {
            string rutaApi = "http://moregarwebapidemo.centralinformatica.com/api/Facturas/";
            var options = new RestClientOptions(rutaApi)
            {
                MaxTimeout = -1,
            };

            var client = new RestClient(options);
            var request = new RestRequest("PostFactura", Method.Post);

            request.AddHeader("Authorization", "Basic bTBSZWdhUl93NFBpOnZTcFIycFl6Z3hibitUUCtiTkVwUzc=");
            request.AddJsonBody(invoiceData);
            
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                return new(true, "Guardado con exito");
            }

            return new(false, message: response.Content);
        }
    }
}
