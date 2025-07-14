using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nubetico.WebAPI.Application.Modules.Core.Services;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Filters
{
    public class EndpointAuthFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var permisosService = context.HttpContext.RequestServices.GetService<PermisosService>();
            if (permisosService == null)
            {
                context.Result = new ObjectResult(ResponseService.Response<object>(
                    StatusCodes.Status500InternalServerError, null, "Service missing"))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                return;
            }

            var userId = context.HttpContext.User.Claims.FirstOrDefault(user => user.Type == "id")?.Value;
            if (!Guid.TryParse(userId, out var guidUser))
            {
                context.Result = new ObjectResult(ResponseService.Response<object>(
                    StatusCodes.Status401Unauthorized, null, "Invalid user ID"))
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            string endpoint = context.HttpContext.Request.Path;
            string webMethod = context.HttpContext.Request.Method;

            bool? hasPermission = await permisosService.UserHasApiPermissionAsync(guidUser, null, endpoint, webMethod);
            if (hasPermission == false)
            {
                context.Result = new ObjectResult(ResponseService.Response<object>(
                    StatusCodes.Status403Forbidden, null, "Permission denied"))
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            // Continue
            await next();
        }
    }


}
