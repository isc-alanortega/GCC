using Newtonsoft.Json;
using Nubetico.WebAPI.Application.External.Directory.Dto;
using System;
using System.Text;

namespace Nubetico.WebAPI.Application.External.Directory
{
    public partial class DirectoryApiServices
    {
        public async Task<HttpResponseMessage?> GetUsuarioByCredencialesAsync(HttpContent body)
        {
            return await _directoryHttpClient.PostAsync(DirectoryEndpoints.UsuarioValidar, body);
        }

        public async Task<HttpResponseMessage?> GetUsuariosByTenantPaginadosAsync(Guid tenantGuid, int limit, int offset, string? orderBy)
        {
            string endpoint = $"{DirectoryEndpoints.UsuariosPorTenantPaginado}/{tenantGuid}";

            var queryParams = new Dictionary<string, string>
            {
                { "limit", limit.ToString() },
                { "offset", offset.ToString() }
            };

            if (!string.IsNullOrEmpty(orderBy))
                queryParams.Add("orderBy", orderBy);

            var queryString = string.Join("&", queryParams.Select(param => $"{param.Key}={param.Value}"));
            var urlWithParams = $"{endpoint}?{queryString}";

            return await _directoryHttpClient.GetAsync(urlWithParams);
        }

        public async Task<HttpResponseMessage?> GetUsuarioByGuidAsync(Guid guid)
        {
            string endpoint = $"{DirectoryEndpoints.Usuarios}/{guid}";
            return await _directoryHttpClient.GetAsync(endpoint);
        }

        public async Task<HttpResponseMessage?> GetUsuarioByEmailAsync(string email)
        {
            string endpoint = $"{DirectoryEndpoints.Usuarios}/email/{email}";
            return await _directoryHttpClient.GetAsync(endpoint);
        }

        public async Task<HttpResponseMessage?> GetUsuarioByUsernameAsync(string username)
        {
            string endpoint = $"{DirectoryEndpoints.Usuarios}/email/{username}";
            return await _directoryHttpClient.GetAsync(endpoint);
        }

        public async Task<HttpResponseMessage?> PostUsuarioAsync(UsuarioDto usuarioDto)
        {
            var content = new StringContent(
                    JsonConvert.SerializeObject(usuarioDto),
                    Encoding.UTF8,
                    "application/json");

            return await _directoryHttpClient.PostAsync(DirectoryEndpoints.Usuarios, content);
        }

        public async Task<HttpResponseMessage?> PutUsuarioAsync(UsuarioDto usuarioDto)
        {
            var content = new StringContent(
                    JsonConvert.SerializeObject(usuarioDto),
                    Encoding.UTF8,
                    "application/json");

            return await _directoryHttpClient.PutAsync(DirectoryEndpoints.Usuarios, content);
        }

        public async Task<HttpResponseMessage?> PostUsuarioUpdatePasswordAsync(Guid guid, string password)
        {
            var content = new StringContent(
                    JsonConvert.SerializeObject(new { uuid = guid.ToString(), password }),
                    Encoding.UTF8,
                    "application/json");

            return await _directoryHttpClient.PostAsync(DirectoryEndpoints.UsuarioSetPassword, content);
        }
    }
}
