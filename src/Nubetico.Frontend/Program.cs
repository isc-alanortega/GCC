using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Nubetico.Frontend.Components.Shared;
using Nubetico.Frontend.Helpers;
using Nubetico.Frontend.Services.Core;
using Nubetico.Frontend.Services.Core.XmlServices;
using Nubetico.Frontend.Services.PortalProveedores;
using Nubetico.Frontend.Services.ProveedoresFacturas;
using Nubetico.Frontend.Services.ProyectosConstruccion;
using Radzen;
using Radzen.Blazor;

namespace Nubetico.Frontend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            if (builder.HostEnvironment.IsProduction())
                builder.Logging.SetMinimumLevel(LogLevel.Warning);

            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddTransient<HttpClientTenantHandler>();
            builder.Services.AddTransient<HttpClientAuthHandler>();
            builder.Services.AddTransient<HttpClientLanguageHandler>();

            builder.Services.AddHttpClient("ApiClient", client =>
            {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
            }).AddHttpMessageHandler<HttpClientTenantHandler>().AddHttpMessageHandler<HttpClientAuthHandler>().AddHttpMessageHandler<HttpClientLanguageHandler>();

            // Sobreescribir las propiedades por defecto del RadzenDataGrid para agregar localización en etiquetas
            var activator = new RadzenComponentActivator();
            activator.Override(typeof(RadzenDataGrid<>), typeof(NubeticoRadzenDataGrid<>));
            builder.Services.AddSingleton<IComponentActivator>(activator);

            builder.Services.AddScoped<ComponentService>();

            builder.Services.AddRadzenComponents();

            builder.Services.AddCoreServices();
            builder.Services.AddPortalProveedoresServices();
            builder.Services.AddProveedoresFacturasServices();
            builder.Services.AddProyectosConstruccionServices();
            builder.Services.AddXmlServices();

            builder.Services.AddAuthorizationCore();
            var host = builder.Build();

            // Instanciar JS
            var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();

            // Limpiar cache
            await jsRuntime.InvokeVoidAsync("clearCache");

            // Aplicar tema e idioma
            await Configure.SetThemeAndLanguageAsync(host.Services);

            // Validar si ya existe un jwt y este es vigente
            await JwtHelper.ValidateJwtAndHandleExpirationAsync(host.Services);

            await host.RunAsync();
        }
    }
}
