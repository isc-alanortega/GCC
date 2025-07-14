using Microsoft.EntityFrameworkCore;
using Nubetico.DAL.Models.Core;
using Nubetico.Shared.Dto.Core;
using Nubetico.WebAPI.Application.Modules.Core.Models.Static;

namespace Nubetico.WebAPI.Application.Modules.Core.Services
{
    public class ParametrosService
    {
        private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;

        public ParametrosService(IDbContextFactory<CoreDbContext> coreDbContextFactory)
        {
            _coreDbContextFactory = coreDbContextFactory;
        }

        public async Task<ThemeConfigDto> GetTheme()
        {
            using var coreDbContext = _coreDbContextFactory.CreateDbContext();

            var requiredAliases = new[]
            {
                ParametrosTema.UrlCss,
                ParametrosTema.UrlFavicon,
                ParametrosTema.NombreWeb,
                ParametrosTema.UrlWebmanifest,
                ParametrosTema.UrlLogoPrincipal,
                ParametrosTema.UrlLogoLogin,
                ParametrosIdioma.Default,
                ParametrosIdioma.Cambiar
            };

            var parametros = await coreDbContext.Parametros
                .Where(p => requiredAliases.Contains(p.Alias))
                .ToDictionaryAsync(p => p.Alias, p => p.Valor1);

            var result = new ThemeConfigDto
            {
                UrlCss = parametros.TryGetValue(ParametrosTema.UrlCss, out var urlCss) ? urlCss : "/assets/nubetico/img/favicon.png",
                UrlFavicon = parametros.TryGetValue(ParametrosTema.UrlFavicon, out var urlFavicon) ? urlFavicon : "/assets/nubetico/img/favicon.png",
                UrlWebManifest = parametros.TryGetValue(ParametrosTema.UrlWebmanifest, out var urlWebManifest) ? urlWebManifest : "/assets/nubetico/manifest.webmanifest",
                UrlLogoLogin = parametros.TryGetValue(ParametrosTema.UrlLogoPrincipal, out var urlLogoPrincipal) ? urlLogoPrincipal : "/assets/nubetico/img/favicon.png",
                UrlLogoPrincipal = parametros.TryGetValue(ParametrosTema.UrlLogoLogin, out var urlLogoLogin) ? urlLogoLogin : "/assets/nubetico/img/favicon.png",
                NombreWeb = parametros.TryGetValue(ParametrosTema.NombreWeb, out var nombreWeb) ? nombreWeb : "nubetico",
                IdiomaDefault = parametros.TryGetValue(ParametrosIdioma.Default, out var idiomaDefault) ? idiomaDefault : "es-MX",
                CambiarIdioma = parametros.TryGetValue(ParametrosIdioma.Cambiar, out var cambiarIdioma) && cambiarIdioma == "1"
            };

            return result;
        }

    }
}
