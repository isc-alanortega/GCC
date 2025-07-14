using FluentValidation;
using System.Globalization;

namespace Nubetico.WebAPI.Middlewares
{
    public class LocalizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly List<string> _supportedCultures = new() { "es-MX", "en-US" };

        public LocalizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            const string defaultLanguage = "es-MX";

            var userLangs = context.Request.Headers["Accept-Language"].ToString();
            var firstLang = userLangs?.Split(',').FirstOrDefault();

            string selectedLanguage = defaultLanguage;

            if (!string.IsNullOrEmpty(firstLang) && _supportedCultures.Contains(firstLang))
            {
                selectedLanguage = firstLang;
            }

            try
            {
                var culture = new CultureInfo(selectedLanguage);
                ValidatorOptions.Global.LanguageManager.Culture = culture;
            }
            catch (CultureNotFoundException)
            {
                ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo(defaultLanguage);
            }

            await _next(context);
        }
    }
}
