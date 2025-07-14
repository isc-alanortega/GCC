namespace Nubetico.WebAPI.Application.Utils
{
    public class NetworkUtils
    {
        public static string GetClientIpAddress(HttpContext context)
        {
            // Buscar encabezado de nginx
            string? realIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            // Si no se encontró, retornar la IP detectada en la conexión
            if (string.IsNullOrEmpty(realIp))
            {
                return context.Connection.RemoteIpAddress?.ToString() ?? "";
            }

            // Formatear IP encontrada en el encabezado de ngix
            realIp = realIp.Split(',')[0].Trim();
            realIp = realIp.Split(':')[0];

            return realIp;
        }
    }
}
