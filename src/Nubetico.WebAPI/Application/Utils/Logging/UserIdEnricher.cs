using Serilog.Core;
using Serilog.Events;

namespace Nubetico.WebAPI.Application.Utils.Logging
{
    public class UserIdEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserIdEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                //var username = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
                var guidUser = httpContext.User.Claims.FirstOrDefault(user => user.Type == "id")?.Value ?? "Unknown";
                var userIdProperty = propertyFactory.CreateProperty("GuidUsuario", guidUser);
                logEvent.AddPropertyIfAbsent(userIdProperty);
            }
        }
    }
}
