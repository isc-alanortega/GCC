using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (!context.HttpContext.Response.HasStarted)
            {
                var request = context.HttpContext.Request;

                var requestInfo = new
                {
                    request.Method,
                    request.Path,
                    QueryParams = GetQueryParams(request)
                };

                _logger.LogError(context.Exception, "ExceptionFilter: {@RequestInfo}", requestInfo);

                context.Result = new ObjectResult(ResponseService.Response<object>(
                    StatusCodes.Status500InternalServerError, null, context.Exception.Message));

                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }

        private string? GetQueryParams(HttpRequest request)
        {
            return request.Query.Any()
                ? string.Join("&", request.Query.Select(q => $"{q.Key}={q.Value}"))
                : null;
        }
    }
}
