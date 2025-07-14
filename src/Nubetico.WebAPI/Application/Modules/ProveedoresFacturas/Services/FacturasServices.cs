using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.Shared.Dto.ProveedoresFacturas;
using Nubetico.WebAPI.Application.External.ProveedoresFacturas;

namespace Nubetico.WebAPI.Application.Modules.ProveedoresFacturas.Services
{
    public class FacturasServices
    {
        public async Task<PaginatedListDto<FacturaDto>> GetFacturas(ApiFacturaPeticion peticion)
        {
            ApiRespuesta<IEnumerable<FacturaDto>> resp = await FacturasEndpoints.GetFacturas(peticion);
            string tmp = JsonConvert.SerializeObject(resp.resultado.ToList());
            PaginatedListDto<FacturaDto> respuesta = new PaginatedListDto<FacturaDto>();
            respuesta.Data = JsonConvert.DeserializeObject<List<FacturaDto>>(tmp!)!;
            respuesta.RecordsTotal = respuesta.Data.Count;
            respuesta.RecordsFiltered = respuesta.Data.Count;
            return (respuesta);

            //Si la api externa regresó un error, arrojar una excepción y mandarle el mensaje del error reportado por la api externa
        }
    }
}