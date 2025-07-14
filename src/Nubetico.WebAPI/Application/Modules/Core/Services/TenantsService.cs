using Newtonsoft.Json;
using Nubetico.Shared.Dto.Common;
using Nubetico.WebAPI.Application.External.Directory;
using Nubetico.WebAPI.Application.External.Directory.Dto;
using System.Net;

namespace Nubetico.WebAPI.Application.Modules.Core.Services
{
    public class TenantsService
    {
        private readonly DirectoryApiServices _directoryApiServices;

        public TenantsService(DirectoryApiServices directoryApiServices)
        {
            _directoryApiServices = directoryApiServices;
        }

        public async Task<List<TenantDto>?> GetByInstanciaAsync(string instanciaUUID)
        {
            var response = await _directoryApiServices.GetTenantsByInstanciaAsync(instanciaUUID);

            if (response.StatusCode == HttpStatusCode.BadRequest)
                throw new Exception("La solicitud a Directory fue erronea");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception("Se produjo un error en Directory");

            var responseContent = await response.Content.ReadAsStringAsync();
            var dataResult = JsonConvert.DeserializeObject<BaseResponseDto<List<TenantDto>>>(responseContent);

            return dataResult != null ? dataResult.Data : null;
        }
    }
}
