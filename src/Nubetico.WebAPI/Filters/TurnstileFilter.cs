using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Nubetico.WebAPI.Application.Utils;

namespace Nubetico.WebAPI.Filters
{
    public class TurnstileFilter : Attribute, IAsyncAuthorizationFilter
    {
        // Configuracion de turnstile
        private const string TurnstileTokenHeader = "x-turnstile-token";
        private const string TurnstileSecretKeyConfigPath = "TurnstileConfig:SecretKey";

        // Posibles errores
        private const string ErrorTokenMissing = "Token de Turnstile no proporcionado.";
        private const string ErrorInvalidConfig = "Configuración de Turnstile no válida.";
        private const string ErrorInvalidToken = "Token de Turnstile no válido.";
        private const string ErrorNetwork = "Error al validar el token: {0}";
        private const string ErrorParsing = "Error al procesar la respuesta: {0}";

        private readonly IConfiguration _configuration;

        public TurnstileFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(TurnstileTokenHeader, out var turnstileToken) || string.IsNullOrEmpty(turnstileToken))
            {
                var response = ResponseService.Response<object>(StatusCodes.Status401Unauthorized, null, ErrorTokenMissing);
                context.Result = new UnauthorizedObjectResult(response);
                return;
            }

            string secretKey = _configuration[TurnstileSecretKeyConfigPath] ?? "";
            if (string.IsNullOrEmpty(secretKey))
            {
                var response = ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, ErrorInvalidConfig);
                context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status500InternalServerError };
                return;
            }

            try
            {
                using var httpClient = new HttpClient();
                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("secret", secretKey),
                new KeyValuePair<string, string>("response", turnstileToken)
            });

                var httpResponse = await httpClient.PostAsync("https://challenges.cloudflare.com/turnstile/v0/siteverify", content);
                var result = await httpResponse.Content.ReadAsStringAsync();

                var jsonResult = JsonConvert.DeserializeObject<TurnstileResponse>(result);
                if (jsonResult == null || !jsonResult.Success)
                {
                    var response = ResponseService.Response<object>(StatusCodes.Status400BadRequest, null, ErrorInvalidToken);
                    context.Result = new BadRequestObjectResult(response);
                    return;
                }
            }
            catch (HttpRequestException ex)
            {
                var response = ResponseService.Response<object>(StatusCodes.Status503ServiceUnavailable, null, string.Format(ErrorNetwork, ex.Message));
                context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status503ServiceUnavailable };
                return;
            }
            catch (JsonException ex)
            {
                var response = ResponseService.Response<object>(StatusCodes.Status500InternalServerError, null, string.Format(ErrorParsing, ex.Message));
                context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status500InternalServerError };
                return;
            }
        }

        private class TurnstileResponse
        {
            public bool Success { get; set; }
        }
    }
}


