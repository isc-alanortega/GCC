using Newtonsoft.Json;
using Nubetico.Shared.Dto.ProveedoresFacturas;
using RestSharp;

namespace Nubetico.WebAPI.Application.External.ProveedoresFacturas
{
    public class FacturasEndpoints
    {
        public static async Task<ApiRespuesta<IEnumerable<FacturaDto>>> GetFacturas(ApiFacturaPeticion peticion)
        {
            string rutaApi = "http://moregarwebapidemo.centralinformatica.com";
            string parametros = "/api/Facturas/PostObtenerFacturas";
            var options = new RestClientOptions(rutaApi)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest(parametros, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Basic bTBSZWdhUl93NFBpOnZTcFIycFl6Z3hibitUUCtiTkVwUzc=");
            var body = JsonConvert.SerializeObject(peticion);

            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(request);

            ApiRespuesta<IEnumerable<FacturaDto>> respuesta = JsonConvert.DeserializeObject<ApiRespuesta<IEnumerable<FacturaDto>>>(response.Content!)!;
            return (respuesta);
        }
    }
}
