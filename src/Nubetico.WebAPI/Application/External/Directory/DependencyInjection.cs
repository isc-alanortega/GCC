namespace Nubetico.WebAPI.Application.External.Directory
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDirectoryServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient("nb.directory", client =>
            {
                client.BaseAddress = new Uri(config["Directory:Url"] ?? "");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("X-API-KEY", config["Directory:ApiKey"] ?? "");
            });

            services.AddTransient<DirectoryApiServices>();

            return services;
        }
    }
}
