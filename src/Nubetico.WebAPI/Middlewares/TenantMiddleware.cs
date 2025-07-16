using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Nubetico.WebAPI.Application.External.Directory.Dto;
using Nubetico.WebAPI.Application.Modules.Core.Models;
using Nubetico.WebAPI.Application.Modules.Core.Services;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Middlewares
{
    public class TenantMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly RequestDelegate _next;
        private readonly TenantConnectionService _tenantConnectionService;
        private readonly TenantsService _tenantsService;

        public TenantMiddleware(IConfiguration configuration,
                                IMemoryCache cache,
                                RequestDelegate next,
                                TenantConnectionService tenantConnectionService,
                                TenantsService tenantsService)
        {
            _configuration = configuration;
            _cache = cache;
            _next = next;
            _tenantConnectionService = tenantConnectionService;
            _tenantsService = tenantsService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            // Header para identificar el tenant
#if (DEBUG)
            string hostHeader = "Host";
#else
            string hostHeader = "X-SITE";
#endif

            if (!context.Request.Headers.TryGetValue(hostHeader, out var tenantUrl))
            {
                await _next(context);
                return;
            }

            // Identificador de la instancia de nubetico
            var instanciaUUID = _configuration["Directory:InstanciaUUID"] ?? "";
            int cacheDurationMinutes = int.TryParse(_configuration["Directory:CacheDurationMinutes"], out var duration) ? duration : 0;

            // Obtener los tenants que se encuentran en cache
            var cacheKey = $"tenants_{instanciaUUID}";
            if (!_cache.TryGetValue(cacheKey, out List<TenantDto>? tenantsInstancia))
            {
                // Si no está en cache, llamar al servicio y almacenarlo en la cache
                tenantsInstancia = await _tenantsService.GetByInstanciaAsync(instanciaUUID);

                if (tenantsInstancia != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheDurationMinutes),
                    };
                    _cache.Set(cacheKey, tenantsInstancia, cacheOptions);
                }
            }

            // Aquí se puede cambiar la url para pruebas.
            // tenantUrl = "clientes.logmex.global";

            TenantDto? tenant = tenantsInstancia?.FirstOrDefault(m => m.WebUrls.Any(url => url.Equals(tenantUrl)));

            if (tenant == null || tenant.Habilitado == false)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(ResponseService.Response<object>(StatusCodes.Status404NotFound, null, "Tenant not found")));
                return;
            }

            TenantModel tenantFound = new TenantModel
            {
                TenantGuid = tenant.UUID ?? Guid.Empty,
                Name = tenant.Nombre ?? string.Empty,
                Description = tenant.Descripcion ?? string.Empty,
                ConnectionString = tenant.CadenaConexion ?? string.Empty,
                TenantUrl = tenantUrl
            };

            // Si el usuario se encuentra autenticado por JWT, verificar que el JWT se haya firmado para el tenant
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                string? tenantGuid = context.User.Claims.FirstOrDefault(user => user.Type == "tenant-id")?.Value;
                if (tenantFound.TenantGuid.ToString() != tenantGuid)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(ResponseService.Response<object>(StatusCodes.Status401Unauthorized, null, "Jwt not valid for this tenant")));
                    return;
                }
            }

            _tenantConnectionService.SetTenant(tenantFound);

            await _next(context);

            _tenantConnectionService.ClearTenant();
        }
    }
}
