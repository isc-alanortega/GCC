namespace Nubetico.WebAPI.Middlewares
{
    public class PermissionsMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //var usuarioId = context.User.Claims.FirstOrDefault(c => c.Type == "UsuarioId")?.Value;

            //if (!string.IsNullOrEmpty(usuarioId))
            //{
            //    var endpoint = context.Request.Path;
            //    var tienePermiso = await permisosService.ValidarPermisoAsync(int.Parse(usuarioId), endpoint);

            //    if (!tienePermiso)
            //    {
            //        context.Response.StatusCode = StatusCodes.Status403Forbidden;
            //        await context.Response.WriteAsync("Acceso denegado");
            //        return;
            //    }
            //}

            await _next(context);
        }
    }
}
